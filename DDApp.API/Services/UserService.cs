using AutoMapper;
using AutoMapper.QueryableExtensions;
using DDApp.API.Models;
using DDApp.DAL;
using Microsoft.EntityFrameworkCore;

namespace DDApp.API.Services
{
    public class UserService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public UserService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task CreateUser(CreateUserModel model)
        {
            var dbUser = _mapper.Map<DDApp.DAL.Entites.User>(model);
            await _context.Users.AddAsync(dbUser);
            await _context.SaveChangesAsync();
        }

        public async Task<List<UserModel>> GetUser()
        {
            return await _context.Users.AsNoTracking().ProjectTo<UserModel>(_mapper.ConfigurationProvider).ToListAsync();
        }
    }
}
