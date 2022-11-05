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
using DDApp.DAL.Entites;
using DDApp.Common.Exceptions;

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

        public async Task<AttachModel> GetUserAvatar(Guid userId)
        {
            var user = await GetUserById(userId);
            if (user == null)
            {
                throw new UserNotFoundException("User not found");
            }

            var attach = _mapper.Map<AttachModel>(user.Avatar);
            if(attach == null)
            {
                throw new AvatarNotFoundException("Avatar not found");
            }
            
            return attach;
        } 

        public async Task CreateUser(CreateUserModel model)
        {
            var dbUser = _mapper.Map<DDApp.DAL.Entites.User>(model);
            await _context.Users.AddAsync(dbUser);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid userId)
        {
            var user = await GetUserById(userId);

            if(user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        private async Task<User> GetUserById(Guid id)
        {
            var user = await _context.Users.Include(x => x.Avatar).FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                throw new UserNotFoundException("User not found");
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
                throw new UserNotFoundException("User not found");
            }

            if (!DDApp.Common.HashHelper.Verify(password, user.PasswordHash))
            {
                throw new WrongPasswordException("Wrong password");
            }

            return user;
        }

        private TokenModel GenerateTokens(UserSession session)
        {
            var dtNow = DateTime.Now;

            var jwt = new JwtSecurityToken(
                issuer: _config.Issuer,
                audience: _config.Audience,
                notBefore: dtNow,
                claims: new Claim[] {
                new Claim("id", session.User.Id.ToString()),
                new Claim("sessionId", session.Id.ToString()),
                new Claim(ClaimsIdentity.DefaultNameClaimType, session.User.Name),
            },
                expires: DateTime.Now.AddMinutes(_config.LifeTime),
                signingCredentials: new SigningCredentials(_config.SymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var refresh = new JwtSecurityToken(
                notBefore: dtNow,
                claims: new Claim[] {
                new Claim("refreshToken", session.RefreshToken.ToString()),
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

            var session = await _context.UserSessions.AddAsync(new UserSession 
            {
                User = user,
                RefreshToken = Guid.NewGuid(),
                Created = DateTime.UtcNow,
                Id = new Guid()
            });

            await _context.SaveChangesAsync();

            return GenerateTokens(session.Entity);
        }

        private async Task<UserSession> GetSessionByRefreshToken(Guid id)
        {
            var session = await _context.UserSessions.Include(x => x.User).FirstOrDefaultAsync(x => x.RefreshToken == id);

            if(session == null)
            {
                throw new SessionNotFoundException("Session is not found");
            }

            return session;
        }

        public async Task<UserSession> GetSessionById(Guid id)
        {
            var session = await _context.UserSessions.FirstOrDefaultAsync(x => x.Id == id);

            if(session == null)
            {
                throw new SessionNotFoundException("Session is not found");
            }

            return session;
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

            if(principal.Claims.FirstOrDefault(x => x.Type == "refreshToken")?.Value is String refreshTokenString
                && Guid.TryParse(refreshTokenString, out var refreshTokenId))
            {
                var session = await GetSessionByRefreshToken(refreshTokenId);
                if (!session.IsActive)
                {
                    throw new SessionNotActiveException("session is not active");
                }

                session.RefreshToken = Guid.NewGuid();
                await _context.SaveChangesAsync();


                return GenerateTokens(session);
            }
            else
            {
                throw new SecurityTokenException("Invalid token");
            }
        }
    }
}
