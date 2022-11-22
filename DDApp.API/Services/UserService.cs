using AutoMapper;
using DDApp.API.Models;
using DDApp.DAL;
using Microsoft.EntityFrameworkCore;
using DDApp.DAL.Entites;
using DDApp.Common.Exceptions;
using DDApp.Common.Exceptions.NotFound;
using DDApp.Common.Exceptions.Forbidden;

namespace DDApp.API.Services
{
    public class UserService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        
        private readonly AttachService _attachService;

        public UserService(IMapper mapper, DataContext context,
            AttachService attachService)
        {
            _mapper = mapper;
            _context = context;
            _attachService = attachService;
        }


        public async Task CreateUser(CreateUserModel model)
        {
            if (await CheckUserExist(model.Email))
            {
                throw new UserCreationForbiddenException();
            }

            var dbUser = _mapper.Map<User>(model);
            await _context.Users.AddAsync(dbUser);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid userId)
        {
            var user = await GetUserById(userId);

            if(user != null)
            {
                user.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<UserWithLinkModel>?> GetUsers()
            =>  await _context.Users
                .AsNoTracking()
                .Where(x => x.IsActive)
                .Include(x => x.Avatar)
                .Select(x => _mapper.Map<User, UserWithLinkModel>(x))
                .ToListAsync();

        public async Task<UserWithLinkModel> GetUser(Guid id)
            =>_mapper.Map<UserWithLinkModel>(await GetUserById(id));

        public async Task AddAvatarToUser(Guid userId, MetadataModel meta)
        {
            var user = await GetUserById(userId);

            var filePath = _attachService.CopyImageFile(meta);

            var avatar = new Avatar
            {
                User = user,
                Author = user,
                MimeType = meta.MimeType,
                FilePath = filePath,
                Size = meta.Size,
                Name = meta.Name,
            };
            user.Avatar = avatar;

            await _context.SaveChangesAsync();
        }


        private async Task<bool> CheckUserExist(string email)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower());
            if (user == null || user.IsActive == false)
            {
                return false;
            }

            return true;
        }

        private async Task<User> GetUserById(Guid id)
        {
            var user = await _context.Users.Include(x => x.Avatar).FirstOrDefaultAsync(x => x.Id == id);

            if (user == null || user.IsActive == false || user == default)
            {
                throw new UserNotFoundException();
            }

            return user;
        }

    }
}
