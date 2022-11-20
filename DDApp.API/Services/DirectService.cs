using AutoMapper;
using DDApp.API.Models.Direct;
using DDApp.Common.Exceptions;
using DDApp.DAL;
using DDApp.DAL.Entites;
using Microsoft.EntityFrameworkCore;

namespace DDApp.API.Services
{
    public class DirectService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly AttachService _attachService;

        public DirectService(DataContext context, IMapper mapper, AttachService attachService)
        {
            _context = context;
            _mapper = mapper;
            _attachService = attachService;
        }

        public async Task<List<DirectModel>?> GetUserDirects(Guid userId)
        {
            var directIds = await _context.DirectMembers.Where(x => x.UserId == userId).ToListAsync();

            var directs = await _context.Directs
                .Include(x => x.DirectImage)
                .Include(x => x.DirectMembers)
                .Select(x => _mapper.Map<Direct, DirectRequestWithSenderModel>(x))
                .ToListAsync();

            var resDirects = new List<DirectModel>();

            directs.ForEach(async x =>
            {
                x.Recipient = (await _context.DirectMembers.FirstAsync(x => x.DirectId == x.DirectId && x.UserId != userId)).User;
                x.DirectImage = await _attachService.GetUserAvatarByUserId(x.Recipient.Id);

                resDirects.Add(_mapper.Map<DirectModel>(directs));
            });

            return resDirects;
        }

        public async Task CreateDirectWithUser(Guid currentUserId, Guid recipientId)
        {
            if (await CheckUserExistById(currentUserId) == false || await CheckUserExistById(recipientId) == false)
            {
                throw new UserException("User not found");
            }

            var directExt = await _context.Directs
                .FirstOrDefaultAsync(
                    x =>
                    x.DirectMembers.First(y => y.UserId == currentUserId).DirectId ==
                    x.DirectMembers.First(y => y.UserId == recipientId).DirectId
                );

            if(directExt != null)
            {
                throw new Exception("Direct already exists");
            }

            var direct = await _context.Directs.AddAsync(new Direct
            {
                DirectId = new Guid(),
                DirectTitle = "local",
            });

            await _context.DirectMembers.AddAsync(new DirectMembers
            {
                DirectId = direct.Entity.DirectId,
                UserId = currentUserId,
            });

            await _context.DirectMembers.AddAsync(new DirectMembers
            {
                DirectId = direct.Entity.DirectId,
                UserId = recipientId,
            });

            await _context.SaveChangesAsync();
        }

        public async Task SendDirectMessage(CreateDirectMessageModel model, Guid sender)
        {
            if(model.Files == null && model.Message == null)
            {
                throw new Exception("One field must be filled");
            }

            var message = (await _context.DirectMessages.AddAsync(new DirectMessages
            {
                DirectMessageId = new Guid(),
                DirectId = model.DirectId,
                DirectMessage = model.Message,
                SenderId = sender,
            })).Entity;

            if (model.Files != null) 
            {
                model.Files.ForEach(async x =>
                {
                    var filePath = _attachService.CopyImageVideoFile(x);

                    await _context.DirectFiles.AddAsync(new DirectFiles
                    {
                        DirectMessagesId = message.DirectMessageId,
                        Author = await _context.Users.FirstAsync(y => y.Id == sender),
                        Name = x.Name,
                        MimeType = x.MimeType,
                        FilePath = filePath,
                        Size = x.Size,
                    });
                });
            }

            await _context.SaveChangesAsync();
        }


        private async Task<bool> CheckUserExistById(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null || user.IsActive == false || user == default)
            {
                return false;
            }

            return true;
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
    }
}
