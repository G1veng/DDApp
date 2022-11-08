using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DDApp.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DDApp.API.Services.AuthService _authService;

        public AuthController(DDApp.API.Services.AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<DDApp.API.Models.TokenModel> Token(DDApp.API.Models.TokenRequestModel tokenModel) 
            => await _authService.GetToken(tokenModel.Login, tokenModel.Password);

        [HttpPost]
        public async Task<DDApp.API.Models.TokenModel> RefreshToken(DDApp.API.Models.RefreshTokenRequestModel model)
            => await _authService.GetTokenByRefreshToken(model.RefreshToken);
    }
}
