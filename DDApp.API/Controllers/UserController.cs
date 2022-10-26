using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DDApp.API.Models;
using AutoMapper;
using DDApp.DAL;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

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
        public async Task CreateUser(CreateUserModel model) => await _userService.CreateUser(model);

        [HttpGet]
        public async Task<List<UserModel>> GetUser() => await _userService.GetUser();
    }
}
