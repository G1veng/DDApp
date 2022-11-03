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

        public UserController(DDApp.API.Services.UserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task CreateUser(CreateUserModel model) 
        {
            if (await _userService.CheckUserExist(model.Email))
                throw new Exception("User is exist");
            await _userService.CreateUser(model);
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
