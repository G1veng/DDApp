using DDApp.API.Models.Direct;
using DDApp.API.Services;
using DDApp.Common.Consts;
using DDApp.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DDApp.Common.Extensions;

namespace DDApp.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DirectController : ControllerBase
    {
        DirectService _directService;

        public DirectController(DirectService directService, LinkGeneratorService linkGeneratorService)
        {
            _directService = directService;
            linkGeneratorService.DirectImageLinkGenerator =
                x => Url.ControllerAction<AttachController>(nameof(AttachController.GetDirectPictureByAttchId), new { directPictureId = x?.Id });
        }

        [HttpGet]
        [Authorize]
        public async Task CreateDirect(Guid userId)
            => await _directService.CreateDirectWithUser(GetCurrentUserGuid(), userId);

        [HttpPost]
        [Authorize]
        public async Task SendDirectMessage(CreateDirectMessageModel model)
            => await _directService.SendDirectMessage(model, GetCurrentUserGuid());

        [HttpGet]
        [Authorize]
        public async Task<List<DirectModel>?> GetUserDirects()
            => await _directService.GetUserDirects(GetCurrentUserGuid());


        private Guid GetCurrentUserGuid()
        {
            var userIdString = User.Claims.FirstOrDefault(x => x.Type == Claims.Id)?.Value;

            if (!Guid.TryParse(userIdString, out var userId))
            {
                throw new AuthorizationException("You are not authorized");
            }
            else
            {
                return userId;
            }
        }
    }
}
