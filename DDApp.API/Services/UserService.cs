using AutoMapper;
using AutoMapper.QueryableExtensions;
using DDApp.API.Models;
using DDApp.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using DDApp.API.Configs;

namespace DDApp.API.Services
{
    public class UserService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly AuthConfig _config;

        public UserService(IMapper mapper, DataContext context, IOptions<AuthConfig> config)
        {
            _mapper = mapper;
            _context = context;
            _config = config.Value;
        }

        public async Task<bool> CheckUserExist(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower()) == null ? false : true;
        }

        public async Task CreateUser(CreateUserModel model)
        {
            var dbUser = _mapper.Map<DDApp.DAL.Entites.User>(model);
            await _context.Users.AddAsync(dbUser);
            await _context.SaveChangesAsync();
        }

        private async Task<DDApp.DAL.Entites.User> GetUserById(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            return user;
        }

        public async Task<List<UserModel>> GetUsers()
        {
            return await _context.Users.AsNoTracking().ProjectTo<UserModel>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<DDApp.API.Models.UserModel> GetUser(Guid id)
        {
            var user = await GetUserById(id);

            return _mapper.Map<DDApp.API.Models.UserModel>(user);
        }

        private async Task<DDApp.DAL.Entites.User> GetByCredential(string login, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == login.ToLower());
            if (user == null)
            {
                throw new Exception("User not found");
            }

            if (!DDApp.Common.HashHelper.Verify(password, user.PasswordHash))
            {
                throw new Exception("Wrong password");
            }

            return user;
        }

        private TokenModel GenerateTokens(DDApp.DAL.Entites.User user)
        {
            var dtNow = DateTime.Now;

            var jwt = new JwtSecurityToken(
                issuer: _config.Issuer,
                audience: _config.Audience,
                notBefore: dtNow,
                claims: new Claim[] {
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
            },
                expires: DateTime.Now.AddMinutes(_config.LifeTime),
                signingCredentials: new SigningCredentials(_config.SymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var refresh = new JwtSecurityToken(
                notBefore: dtNow,
                claims: new Claim[] {
                new Claim("id", user.Id.ToString()),
            },
                expires: DateTime.Now.AddHours(_config.LifeTime),
                signingCredentials: new SigningCredentials(_config.SymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                );

            var encodedRefresh = new JwtSecurityTokenHandler().WriteToken(refresh);

            return new TokenModel(encodedJwt, encodedRefresh);

        }

        public async Task<TokenModel> GetToken(string login, string password)
        {
            var user = await GetByCredential(login, password);

            return GenerateTokens(user);
        }

        public async Task<TokenModel> GetTokenByRefreshToken(string refreshToken)
        {
            var validParams = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                IssuerSigningKey = _config.SymmetricSecurityKey()
            };

            var principal = new JwtSecurityTokenHandler().ValidateToken(refreshToken, validParams, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtToken 
                || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            if(principal.Claims.FirstOrDefault(x => x.Type == "id")?.Value is String userIdString
                && Guid.TryParse(userIdString, out var userId))
            {
                var user = await GetUserById(userId);

                return GenerateTokens(user);
            }
            else
            {
                throw new SecurityTokenException("Invalid token");
            }
        }
    }
}
