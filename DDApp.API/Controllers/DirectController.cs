using DDApp.API.Models.Direct;
using DDApp.API.Services;
using DDApp.Common.Consts;
using DDApp.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DDApp.Common.Extensions;
using DDApp.Common.Exceptions.Authorization;

namespace DDApp.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Api")]
    public class DirectController : ControllerBase
    {
        DirectService _directService;

        public DirectController(DirectService directService, LinkGeneratorService linkGeneratorService)
        {
            _directService = directService;
            linkGeneratorService.DirectImageLinkGenerator =
                x => Url.ControllerAction<AttachController>(nameof(AttachController.GetDirectPictureByAttchId), new { directPictureId = x?.Id });
            linkGeneratorService.DirectFilesLinkGenerator =
                x => Url.ControllerAction<AttachController>(nameof(AttachController.GetDirectFileByAttchId), new { directFileId = x?.Id });
            linkGeneratorService.DirectGroupImageLinkGenerator =
                x => Url.ControllerAction<AttachController>(nameof(AttachController.GetDirectPictureByAttchId), new { directPictureId = x?.Id});
        }

        [HttpPost]
        [Authorize]
        public async Task CreateDirect(Guid userId)
            => await _directService.CreateDirectWithUser(GetCurrentUserGuid(), userId);

        [HttpPost]
        [Authorize]
        public async Task CreateDirectMessage(CreateDirectMessageModel model)
            => await _directService.CreateDirectMessage(model, GetCurrentUserGuid());

        [HttpGet]
        [Authorize]
        public async Task<List<DirectModel>?> GetUserDirects()
            => await _directService.GetUserDirects(GetCurrentUserGuid());

        [HttpGet]
        [Authorize]
        public async Task<DirectRequestModel> GetUserDirect(Guid directId)
            => await _directService.GetUserDirect(directId, GetCurrentUserGuid());

        [HttpPost]
        [Authorize]
        public async Task CreateDirectGroup(CreateDirectGroupModel model)
            => await _directService.CreateDirectGroup(model, GetCurrentUserGuid());


        private Guid GetCurrentUserGuid()
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
