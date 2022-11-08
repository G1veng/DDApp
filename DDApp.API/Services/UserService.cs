using AutoMapper;
using AutoMapper.QueryableExtensions;
using DDApp.API.Models;
using DDApp.DAL;
using Microsoft.EntityFrameworkCore;
using DDApp.DAL.Entites;
using DDApp.Common.Exceptions;
using DDApp.API.Models.User;

namespace DDApp.API.Services
{
    public class UserService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        
        private readonly AttachService _attachService;
        private Func<Avatar?, string?>? _linkGenerator;

        public void SetLinkGenerator(Func<Avatar?, string?>? linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }

        public UserService(IMapper mapper, DataContext context,
            AttachService attachService)
        {
            _mapper = mapper;
            _context = context;
            _attachService = attachService;
        }

        public async Task<bool> CheckUserExist(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower());
            if(user == null || user.IsActive == false)
            {
                return false;
            }

            return true;
        }

        public async Task<AttachModel> GetUserAvatar(Guid userId)
        {
            var user = await GetUserById(userId);
            if (user == null || user.IsActive == false)
            {
                throw new UserException("User not found");
            }

            var attach = _mapper.Map<AttachModel>(user.Avatar);
            if(attach == null)
            {
                throw new Common.Exceptions.FileException("Avatar not found");
            }
            
            return attach;
        } 

        public async Task CreateUser(CreateUserModel model)
        {
            var dbUser = _mapper.Map<DDApp.DAL.Entites.User>(model);
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

        private async Task<User> GetUserById(Guid id)
        {
            var user = await _context.Users.Include(x => x.Avatar).FirstOrDefaultAsync(x => x.Id == id);
            if (user == null || user.IsActive == false)
            {
                throw new UserException("User not found");
            }

            return user;
        }

        public async Task<List<UserWithLinkModel>> GetUsers()
        {
            var resUsers = new List<UserWithLinkModel>();
            var users = await _context.Users.AsNoTracking()
                .Where(x => x.IsActive)
                .Include(x => x.Avatar)
                .ProjectTo<UserRequestModel>(_mapper.ConfigurationProvider)
                .ToListAsync();

            users.ForEach(x => 
            {
                x.LinkGenerator = _linkGenerator;
                resUsers.Add(_mapper.Map<UserWithLinkModel>(x));
            });

            return resUsers;
        }

        public async Task<DDApp.API.Models.UserWithLinkModel> GetUser(Guid id)
        {
            var user = _mapper.Map<DDApp.API.Models.User.UserRequestModel>(await GetUserById(id));
            user.LinkGenerator = _linkGenerator;

            return _mapper.Map<UserWithLinkModel>(user);
        }

        public async Task AddAvatarToUser(Guid userId, MetadataModel meta)
        {
            var user = await _context.Users.Include(x => x.Avatar).FirstOrDefaultAsync(x => x.Id == userId);

            if (user != null)
            {

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
        }
    }
}
