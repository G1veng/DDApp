using DDApp.API.Models.Push;
using DDApp.API.Services;
using DDApp.Common.Consts;
using DDApp.Common.Exceptions.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DDApp.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class PushController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly GooglePushService _googlePushService;

        public PushController(UserService userService, GooglePushService googlePushService)
        {
            _userService = userService;
            _googlePushService = googlePushService;
        }

        [HttpPost]
        public async Task Subscribe(PushTokenModel model)
        {
            var userId = GetCurrentUserId();
            if(userId != default)
            {
                await _userService.SetPushToken(userId, model.Token);
            }
        }

        [HttpDelete]
        public async Task Unsubscribe()
        {
            var userId = GetCurrentUserId();
            if (userId != default)
            {
                await _userService.SetPushToken(userId);
            }
        }

        [HttpPost]
        public async Task<List<string>> SendPush(SendPushModel model)
        {
            var res = new List<string>();
            var userId = model.UserId ?? GetCurrentUserId();
            if(userId != default)
            {
                var token = await _userService.GetPushToken(userId);
                if(token != default)
                {
                    res = _googlePushService.SendNotification(token, model.Push);
                }
            }

            return res;
        }


        private Guid GetCurrentUserId()
        {
            var userIdString = User.Claims.FirstOrDefault(x => x.Type == Claims.Id)?.Value;

            if (!Guid.TryParse(userIdString, out var userId))
            {
                throw new UserAuthorizationException();
            }
            else
            {
                return userId;
            }
        }
    }
}
