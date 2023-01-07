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
    [Authorize]
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
            linkGeneratorService.AvatarDirectImageLinkGenerator =
                x => Url.ControllerAction<AttachController>(nameof(AttachController.GetDirectPictureByAttchId), new { directPictureId = x?.Id });
        }

        [HttpPost]
        public async Task CreateDirect(CreateDirectModel model)
            => await _directService.CreateDirectWithUser(GetCurrentUserGuid(), model);

        [HttpPost]
        public async Task CreateDirectMessage(CreateDirectMessageModel model)
            => await _directService.CreateDirectMessage(model, GetCurrentUserGuid());

        [HttpGet]
        public async Task<DirectModel?> GetDirectWithUser(Guid userId)
            => await _directService.GetDirectWithUser(GetCurrentUserGuid(), userId);

        [HttpGet]
        public async Task<List<DirectModel>?> GetUserDirects(int skip = 0, int take = 10)
            => await _directService.GetUserDirects(GetCurrentUserGuid(), take, skip);

        [HttpGet]
        public async Task<DirectModel?> GetUserDirect(Guid directId)
            => await _directService.GetUserDirect(directId, GetCurrentUserGuid());

        [HttpGet]
        public async Task<List<DirectMessageModel>?> GetDirectMessage(Guid directId, int skip = 0,
            int take = 10,
            DateTimeOffset? lastDirectMessageCreated = null)
            => await _directService.GetDirectMessage(GetCurrentUserGuid(), directId, skip, take, lastDirectMessageCreated);


        [HttpPost]
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
