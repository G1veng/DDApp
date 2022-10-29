using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DDApp.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DDApp.API.Services.UserService _userService;

        public AuthController(DDApp.API.Services.UserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<DDApp.API.Models.TokenModel> Token(DDApp.API.Models.TokenRequestModel tokenModel) 
            => await _userService.GetToken(tokenModel.Login, tokenModel.Password);

        [HttpPost]
        public async Task<DDApp.API.Models.TokenModel> RefreshToken(DDApp.API.Models.RefreshTokenRequestModel model)
            => await _userService.GetTokenByRefreshToken(model.RefreshToken);
    }
}
