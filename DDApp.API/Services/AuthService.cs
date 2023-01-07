using DDApp.API.Configs;
using DDApp.API.Models;
using DDApp.Common.Exceptions;
using DDApp.DAL.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DDApp.Common.Consts;
using DDApp.Common.Exceptions.NotFound;
using DDApp.Common.Exceptions.Authorization;

namespace DDApp.API.Services
{
    public class AuthService
    {
        private readonly AuthConfig _config;
        private readonly DAL.DataContext _context;

        public AuthService(IOptions<AuthConfig> config, DAL.DataContext context)
        {
            _config = config.Value;
            _context = context;
        }

        private TokenModel GenerateTokens(UserSession session)
        {
            var dtNow = DateTime.Now;

            var jwt = new JwtSecurityToken(
                issuer: _config.Issuer,
                audience: _config.Audience,
                notBefore: dtNow,
                claims: new Claim[] {
                new Claim(Claims.Id, session.User.Id.ToString()),
                new Claim(Claims.SessionId, session.Id.ToString()),
                new Claim(ClaimsIdentity.DefaultNameClaimType, session.User.Name),
            },
                expires: DateTime.Now.AddMinutes(_config.LifeTime),
                signingCredentials: new SigningCredentials(_config.SymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var refresh = new JwtSecurityToken(
                notBefore: dtNow,
                claims: new Claim[] {
                new Claim(Claims.RefreshToken, session.RefreshToken.ToString()),
            },
                expires: DateTime.Now.AddHours(_config.LifeTime),
                signingCredentials: new SigningCredentials(_config.SymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                );

            var encodedRefresh = new JwtSecurityTokenHandler().WriteToken(refresh);

            return new TokenModel(encodedJwt, encodedRefresh);

        }

        private async Task<User> GetByCredential(string login, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == login.ToLower());
            if (user == null || user.IsActive == false)
            {
                throw new UserNotFoundException();
            }

            if (!Common.HashHelper.Verify(password, user.PasswordHash))
            {
                throw new PasswordAuthorizetionException();
            }

            return user;
        }

        public async Task<TokenModel> GetToken(string login, string password)
        {
            var user = await GetByCredential(login, password);

            var session = await _context.UserSessions.AddAsync(new UserSession
            {
                User = user,
                RefreshToken = Guid.NewGuid(),
                Created = DateTime.UtcNow,
                Id = Guid.NewGuid()
            });

            await _context.SaveChangesAsync();

            return GenerateTokens(session.Entity);
        }

        private async Task<UserSession> GetSessionByRefreshToken(Guid id)
        {
            var session = await _context.UserSessions.Include(x => x.User).FirstOrDefaultAsync(x => x.RefreshToken == id);

            if (session == null)
            {
                throw new SessionNotFoundException();
            }

            return session;
        }

        public async Task<UserSession> GetSessionById(Guid id)
        {
            var session = await _context.UserSessions.FirstOrDefaultAsync(x => x.Id == id);

            if (session == null)
            {
                throw new SessionNotFoundException();
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

            if (principal.Claims.FirstOrDefault(x => x.Type == "refreshToken")?.Value is String refreshTokenString
                && Guid.TryParse(refreshTokenString, out var refreshTokenId))
            {
                var session = await GetSessionByRefreshToken(refreshTokenId);
                if (!session.IsActive || session == default || session == null)
                {
                    throw new AuthorizationException();
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
