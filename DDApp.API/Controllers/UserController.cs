using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DDApp.API.Models;
using AutoMapper;
using DDApp.DAL;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;

namespace DDApp.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DDApp.API.Services.UserService _userService;
        private readonly DDApp.API.Services.AttachmentsService _attachmentsService;

        public UserController(DDApp.API.Services.UserService userService, DDApp.API.Services.AttachmentsService attachmentsService)
        {
            _userService = userService;
            _attachmentsService = attachmentsService;
        }

        [HttpPost]
        public async Task CreateUser(CreateUserModel model) 
        {
            if (await _userService.CheckUserExist(model.Email))
                throw new Exception("User is exist");
            await _userService.CreateUser(model);
        }

        [HttpPost]
        [Authorize]
        public async Task AddAvatarToUser(MetadataModel model)
        {
            var userIdString = User.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
            if (Guid.TryParse(userIdString, out var userId))
            {
                var tempFi = new FileInfo(Path.Combine(Path.GetTempPath(), model.TempId.ToString()));

                if (!tempFi.Exists)
                {
                    throw new Exception("File not found");
                }
                else
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "attaches", model.TempId.ToString());

                    var destFi = new FileInfo(path);
                    if (destFi.Directory != null && !destFi.Directory.Exists)
                    {
                        destFi.Directory.Create();
                    }

                    System.IO.File.Copy(tempFi.FullName, path, true);

                    await _attachmentsService.AddAvatarToUser(userId, model, path);
                }
            }
            else
            {
                throw new Exception("You are not authorized");
            }
        }

        [HttpGet]
        public async Task<FileResult> GetUserAvatar(Guid userId)
        {
            var attach = await _userService.GetUserAvatar(userId);

            return File(System.IO.File.ReadAllBytes(attach.FilePath), attach.MimeType);
        }

        [HttpGet]
        [Authorize]
        public async Task<List<UserModel>> GetUsers() => await _userService.GetUsers();

        [HttpGet]
        [Authorize]
        public async Task<UserModel> GetCurrentUser() 
        {
            var userIdString = User.Claims.FirstOrDefault(x => x.Type == "id")?.Value;

            if(Guid.TryParse(userIdString, out var userId))
            {
                return await _userService.GetUser(userId);
            }
            else
            {
                throw new Exception("You are not authorized");
            }
        }
    }
}
