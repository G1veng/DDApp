using Microsoft.AspNetCore.Mvc;
using DDApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using DDApp.Common.Exceptions;
using DDApp.Common.Consts;

namespace DDApp.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DDApp.API.Services.UserService _userService;
        private readonly DDApp.API.Services.AttachService _attachmentsService;

        public UserController(DDApp.API.Services.UserService userService, DDApp.API.Services.AttachService attachService)
        {
            userService.SetLinkGenerator(x 
                => Url.Action(nameof(AttachController.GetUserAvatarByAttachId), nameof(AttachController), new { userId = x?.UserId, download = false }));
            _userService = userService;
            _attachmentsService = attachService;
            
        }

        [HttpPost]
        public async Task CreateUser(CreateUserModel model) 
        {
            if (await _userService.CheckUserExist(model.Email))
            {
                throw new UserException("User is exist");
            }
                
            await _userService.CreateUser(model);
        }

        [HttpPost]
        [Authorize]
        public async Task AddAvatarToUser(MetadataModel model)
        {
            var userIdString = User.Claims.FirstOrDefault(x => x.Type == Claims.Id)?.Value;
            if (Guid.TryParse(userIdString, out var userId))
            {
                await _userService.AddAvatarToUser(userId, model);
            }
            else
            {
                throw new AuthorizationException("You are not authorized");
            }
        }

        

        [HttpGet]
        [Authorize]
        public async Task<List<UserWithLinkModel>> GetUsers() => await _userService.GetUsers();

        [HttpGet]
        [Authorize]
        public async Task<UserWithLinkModel> GetCurrentUser() 
        {
            var userIdString = User.Claims.FirstOrDefault(x => x.Type == Claims.Id)?.Value;

            if(Guid.TryParse(userIdString, out var userId))
            {
                return await _userService.GetUser(userId);
            }
            else
            {
                throw new AuthorizationException("You are not authorized");
            }
        }
    }
}
