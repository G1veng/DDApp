using AutoMapper;
using DDApp.API.Models.Direct;
using DDApp.Common.Exceptions;
using DDApp.DAL;
using DDApp.DAL.Entites;
using DDApp.DAL.Entites.DirectDir;
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

        public async Task CreateDirectGroup(CreateDirectGroupModel model, Guid userId)
        {
            model.Members.Add(userId);

            if (model.Members == null || model.Members.Count < 2)
            {
                throw new Exception("Not enough members");
            }

            model.Members.ForEach(async u =>
            {
                if (!(await CheckUserExistById(u)))
                {
                    throw new Exception("User not found");
                }
            });

            if(model.Title == null || model.Title.Length == 0)
            {
                throw new Exception("Group title must be presented");
            }

            var direct = (await _context.Directs.AddAsync(new Direct
            {
                DirectId = new Guid(),
                DirectTitle = model.Title,
                IsDirectGroup = true,
                Created = DateTimeOffset.UtcNow,
            })).Entity;

            if(model.GroupImage != null)
            {
                var filePath = _attachService.CopyImageVideoFile(model.GroupImage);

                await _context.DirectImages.AddAsync(new DAL.Entites.DirectDir.DirectImages
                {
                    DirectId = direct.DirectId,
                    Author = _context.Users.First(y => y.Id == userId),
                    Name = model.GroupImage.Name,
                    MimeType = model.GroupImage.MimeType,
                    FilePath = filePath,
                    Size = model.GroupImage.Size,
                });
            }

            model.Members.ForEach(async x =>
            {
                await _context.DirectMembers.AddAsync(new DirectMembers
                {
                    DirectId = direct.DirectId,
                    UserId = x,
                });
            });

            await _context.SaveChangesAsync();
        }

        public async Task<DirectRequestModel> GetUserDirect(Guid directId, Guid senderId)
        {
            if((await _context.Directs.FirstOrDefaultAsync(x => x.DirectId == directId)) == null)
            {
                throw new Exception("Direct is not existing");
            }

            var res = new DirectRequestModel
            {
                DirectId = directId,
                RecipientId = (await _context.DirectMembers.Include(y => y.User).FirstAsync(y => y.DirectId == directId && y.UserId != senderId)).UserId,
            };

            res.DirectMessages = await _context.DirectMessages
                .Include(x => x.DirectFiles)
                .Include(x => x.User)
                .Select(x => _mapper.Map<DirectMessages, DirectMessageModel>(x))
                .ToListAsync();

            return res;
        }

        public async Task<List<DirectModel>?> GetUserDirects(Guid userId)
        {
            var directs = await _context.Directs
                .Include(x => x.DirectImage)
                .Include(x => x.DirectMembers)
                .Include(x => x.DirectMembers).ThenInclude(x => x.User)
                .Include(x => x.DirectMembers).ThenInclude(x => x.User).ThenInclude(x => x.Avatar)
                .Where(x => x.DirectMembers.Contains(new DirectMembers { DirectId = x.DirectId, UserId = userId }))
                .Select(x => _mapper.Map<Direct, DirectModel>(x))
                .ToListAsync();

            directs.ForEach(async x =>
            {
                if(x.DirectMembers.Count == 1)
                {
                    var userAvatar = await _context.Avatars.FirstOrDefaultAsync(y => y.UserId == x.DirectMembers.First(x => x.DirectMember != userId).DirectMember);

                    if (userAvatar != null)
                    {
                        x.DirectImage = _mapper.Map<DirectImages, DirectImageModel>(new DirectImages
                        {
                            DirectId = x.DirectId,
                            Id = userAvatar.Id,
                            Size = userAvatar.Size,
                            MimeType = userAvatar.MimeType,
                            FilePath = userAvatar.FilePath,
                            Name = userAvatar.Name,
                        });
                    }
                }
            });

            return directs;
        }

        public async Task CreateDirectWithUser(Guid currentUserId, Guid recipientId)
        {
            if (await CheckUserExistById(currentUserId) == false || await CheckUserExistById(recipientId) == false)
            {
                throw new UserNotFoundException();
            }

            var directExt = await _context.Directs
                .Include(x => x.DirectMembers)
                .FirstOrDefaultAsync(
                    x => x.DirectMembers.Count == 2 
                    && x.DirectMembers.Contains(new DirectMembers { UserId = currentUserId, DirectId = x.DirectId})
                    && x.DirectMembers.Contains(new DirectMembers { UserId = recipientId, DirectId = x.DirectId })
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

            _context.DirectMembers.Add(new DirectMembers
            {
                DirectId = direct.Entity.DirectId,
                UserId = currentUserId,
            });

            _context.DirectMembers.Add(new DirectMembers
            {
                DirectId = direct.Entity.DirectId,
                UserId = recipientId,
            });

            await _context.SaveChangesAsync();
        }

        public async Task CreateDirectMessage(CreateDirectMessageModel model, Guid sender)
        {
            if((model.Files == null || model.Files?.Count == null) && (model.Message == null || model.Message == String.Empty))
            {
                throw new Exception("One field must be filled (messege or files)");
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
                model.Files.ForEach(x =>
                {
                    var filePath = _attachService.CopyImageVideoFile(x);

                    _context.DirectFiles.Add(new DirectFiles
                    {
                        DirectMessagesId = message.DirectMessageId,
                        Author = _context.Users.First(y => y.Id == sender),
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
    }
}
