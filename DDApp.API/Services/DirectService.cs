using AutoMapper;
using DDApp.API.Models.Direct;
using DDApp.Common.Exceptions;
using DDApp.Common.Exceptions.Forbidden;
using DDApp.Common.Exceptions.NotFound;
using DDApp.Common.Exceptions.UnprocessableEntity;
using DDApp.DAL;
using DDApp.DAL.Entites;
using DDApp.DAL.Entites.DirectDir;
using Microsoft.EntityFrameworkCore;
using DDApp.Common.Consts;

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

        /// <summary>
        /// Создают директ группу
        /// </summary>
        public async Task CreateDirectGroup(CreateDirectGroupModel model, Guid userId)
        {
            model.Members.Add(userId);

            if (model.Members == null || model.Members.Count < 2)
            {
                throw new MembresCountUnprocessableEntityException();
            }

            model.Members.ForEach(async u =>
            {
                if (!(await CheckUserExistById(u)))
                {
                    throw new UserNotFoundException();
                }
            });

            if(model.Title == default || model.Title.Length == 0)
            {
                throw new GroupTitleUnprocessableEntityException();
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
                var filePath = _attachService.CopyImageFile(model.GroupImage);

                await _context.DirectImages.AddAsync(new DirectImages
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

        /// <summary>
        /// Возвращает дерект пользователя
        /// </summary>
        public async Task<DirectRequestModel> GetUserDirect(Guid directId, Guid senderId)
        {
            if((await _context.Directs.AsNoTracking().FirstOrDefaultAsync(x => x.DirectId == directId)) == null)
            {
                throw new DirectNotFoundException();
            }

            if (!(await CheckUserExistById(senderId)))
            {
                throw new UserNotFoundException();
            }

            var res = new DirectRequestModel
            {
                DirectId = directId,
                RecipientId = (await _context.DirectMembers.AsNoTracking().Include(y => y.User).FirstAsync(y => y.DirectId == directId && y.UserId != senderId)).UserId,
            };

            res.DirectMessages = await _context.DirectMessages
                .AsNoTracking()
                .Include(x => x.DirectFiles)
                .Include(x => x.User)
                .Where(x => x.DirectId == directId)
                .Select(x => _mapper.Map<DirectMessages, DirectMessageModel>(x))
                .ToListAsync();

            return res;
        }

        /// <summary>
        /// Возвращает список директов пользователя
        /// </summary>
        public async Task<List<DirectModel>?> GetUserDirects(Guid userId)
        {
            var directs = await _context.Directs
                .AsNoTracking()
                .Include(x => x.DirectImage)
                .Include(x => x.DirectMembers)
                .Include(x => x.DirectMembers).ThenInclude(x => x.User)
                .Include(x => x.DirectMembers).ThenInclude(x => x.User).ThenInclude(x => x.Avatar)
                .Where(x => x.DirectMembers.Contains(new DirectMembers { DirectId = x.DirectId, UserId = userId }))
                .Select(x => _mapper.Map<Direct, DirectModel>(x))
                .ToListAsync();

            directs.ForEach(d =>
            {
                if(d.DirectMembers.Count == 2)
                {
                    var recipient = d.DirectMembers.First(m => m.DirectMember != userId).DirectMember;

                    var userAvatar = _context.Avatars.AsNoTracking().FirstOrDefault(u => u.UserId == recipient);

                    if (userAvatar != null)
                    {
                        d.DirectImage = _mapper.Map<DirectImages, DirectImageModel>(new DirectImages
                        {
                            DirectId = d.DirectId,
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

        /// <summary>
        /// Создает директ (один на один) с указанным пользователем
        /// </summary>
        public async Task CreateDirectWithUser(Guid currentUserId, Guid recipientId)
        {
            if (await CheckUserExistById(currentUserId) == false || await CheckUserExistById(recipientId) == false)
            {
                throw new UserNotFoundException();
            }

            if((await _context.Directs
                .AsNoTracking()
                .Include(x => x.DirectMembers)
                .FirstOrDefaultAsync(x => x.DirectMembers.Count == 2
                    && x.DirectMembers.Contains(new DirectMembers { UserId = currentUserId, DirectId = x.DirectId })
                    && x.DirectMembers.Contains(new DirectMembers { UserId = recipientId, DirectId = x.DirectId })))
            != null)
            {
                throw new DirectExistsForbiddenException();
            }

            var direct = await _context.Directs.AddAsync(new Direct
            {
                DirectId = new Guid(),
                DirectTitle = DefaultGroupTitle.Title,
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

        /// <summary>
        /// Отправляет сообщение в выбранный директ
        /// </summary>
        public async Task CreateDirectMessage(CreateDirectMessageModel model, Guid sender)
        {
            if((model.Files == null || model.Files?.Count == null) && (model.Message == null || model.Message == String.Empty))
            {
                throw new MessageOrFilesUnprocessableEntityException();
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
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null || user.IsActive == false || user == default)
            {
                return false;
            }

            return true;
        }
    }
}
