using Microsoft.AspNetCore.Mvc;
using DDApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using DDApp.Common.Exceptions;
using DDApp.Common.Consts;
using DDApp.Common.Extensions;
using DDApp.Common.Exceptions.Forbidden;
using DDApp.Common.Exceptions.Authorization;

namespace DDApp.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Api")]
    public class UserController : ControllerBase
    {
        private readonly DDApp.API.Services.UserService _userService;
        private readonly DDApp.API.Services.AttachService _attachmentsService;

        public UserController(DDApp.API.Services.UserService userService, DDApp.API.Services.AttachService attachService,
            Services.LinkGeneratorService linkGeneratorService)
        {
            linkGeneratorService.AvatarLinkGenerator =
                x => Url.ControllerAction<AttachController>(nameof(AttachController.GetUserAvatarByAttachId), new { attachId = x?.Id });
            _userService = userService;
            _attachmentsService = attachService;
        }

        [HttpPost]
        [ApiExplorerSettings(GroupName = "Auth")]
        [AllowAnonymous]
        public async Task CreateUser(CreateUserModel model) 
            => await _userService.CreateUser(model);

        [HttpPost]
        [Authorize]
        public async Task AddAvatarToUser(MetadataModel model)
            => await _userService.AddAvatarToUser(GetCurrentUserGuid(), model);

        [HttpGet]
        [Authorize]
        public async Task<List<UserWithLinkModel>?> GetUsers()
            => await _userService.GetUsers();

        [HttpGet]
        [Authorize]
        public async Task<UserWithLinkModel> GetCurrentUser() 
            => await _userService.GetUser(GetCurrentUserGuid());

        [HttpGet]
        [Authorize]
        public async Task<UserWithLinkModel?> GetUser(string userId)
            => await _userService.GetUser(Guid.Parse(userId));


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
