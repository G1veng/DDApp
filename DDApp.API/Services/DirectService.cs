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
using DDApp.Common.Exceptions.Authorization.SpecificExceptions;

namespace DDApp.API.Services
{
    public class DirectService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly AttachService _attachService;
        private readonly GooglePushService _pushService;
        private readonly AttachService _attchService;

        public DirectService(DataContext context, IMapper mapper, AttachService attachService, GooglePushService pushService, AttachService attchService)
        {
            _context = context;
            _mapper = mapper;
            _attachService = attachService;
            _pushService = pushService;
            _attchService = attchService;
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
                DirectId = model.Id ?? Guid.NewGuid(),
                DirectTitle = model.Title,
                IsDirectGroup = true,
                Created = model.Created ?? DateTimeOffset.UtcNow,
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
        public async Task<DirectModel?> GetUserDirect(Guid directId, Guid senderId)
        {
            var direct = await _context.Directs
                .AsNoTracking()
                .Include(x => x.DirectMembers)
                .Include(x => x.DirectMembers).ThenInclude(x => x.User)
                .Include(x => x.DirectMembers).ThenInclude(x => x.User).ThenInclude(x => x.Avatar)
                .Include(x => x.DirectImage)
                .FirstOrDefaultAsync(x => x.DirectId == directId);

            if (direct == null)
            {
                throw new DirectNotFoundException();
            }

            if (!(await CheckUserExistById(senderId)))
            {
                throw new UserNotFoundException();
            }

            var directMembers = await _context.DirectMembers.Where(x => x.DirectId == directId).ToListAsync();
            if (directMembers.FirstOrDefault(x => x.UserId == senderId) == null)
            {
                throw new AccessAuthorizationException();
            }

            return _mapper.Map<DirectModel>(direct);
        }

        public async Task<List<DirectMessageModel>?> GetDirectMessage(Guid currentUser, Guid directId, int skip, int take, DateTimeOffset? lastDirectMessageCreated = null)
        {
            var direct = (await _context.Directs
                .AsNoTracking()
                .Include(x => x.DirectMembers)
                .Include(x => x.DirectMembers).ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.DirectId == directId));

            if (direct == null)
            {
                throw new DirectNotFoundException();
            }

            var directMembers = await _context.DirectMembers.Where(x => x.DirectId == directId).ToListAsync();
            if (directMembers.FirstOrDefault(x => x.UserId == currentUser) == null)
            {
                throw new AccessAuthorizationException();
            }

            return await _context.DirectMessages
                .AsNoTracking()
                .Where(x => x.DirectId == directId && (lastDirectMessageCreated == null ? true : x.Sended > lastDirectMessageCreated))
                .OrderByDescending(x => x.Sended)
                .Skip(skip)
                .Take(take)
                .Include(x => x.Direct)
                .Include(x => x.DirectFiles)
                .Include(x => x.User)
                .Select(x => _mapper.Map<DirectMessages, DirectMessageModel>(x))
                .ToListAsync();


        }

        public async Task<DirectModel?> GetDirectWithUser(Guid currentUserId, Guid userId)
        {
            var direct = await _context.Directs
                .AsNoTracking()
                .Include(x => x.DirectImage)
                .Include(x => x.DirectMembers)
                .Include(x => x.DirectMembers).ThenInclude(x => x.User)
                .Include(x => x.DirectMembers).ThenInclude(x => x.User).ThenInclude(x => x.Avatar)
                .FirstOrDefaultAsync(x => x.DirectMembers
                    .Contains(new DirectMembers { DirectId = x.DirectId, UserId = currentUserId})
                    && x.DirectMembers
                    .Contains(new DirectMembers { DirectId = x.DirectId, UserId = userId})
                    && x.DirectMembers.Count == 2);
            if(direct == null)
            {
                return null;
            }

            var member = await _context.DirectMembers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.DirectId == direct.DirectId && x.UserId == userId);
            if (member != null)
            {
                var avatar = await _context.Avatars
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.UserId == member.UserId);
                if (avatar != null)
                {
                    var res = _mapper.Map<Avatar, DirectImageModel>(avatar);
                    var dirModel = _mapper.Map<DirectModel>(direct);
                    dirModel.DirectImage = res;

                    return dirModel;
                }
            }

            return _mapper.Map<DirectModel>(direct);
        }

        /// <summary>
        /// Возвращает список директов пользователя
        /// </summary>
        public async Task<List<DirectModel>?> GetUserDirects(Guid userId, int take, int skip)
        {
            var directs = await _context.Directs
                .AsNoTracking()
                .Include(x => x.DirectImage)
                .Include(x => x.DirectMembers)
                .Include(x => x.DirectMembers).ThenInclude(x => x.User)
                .Include(x => x.DirectMembers).ThenInclude(x => x.User).ThenInclude(x => x.Avatar)
                .Where(x => x.DirectMembers.Contains(new DirectMembers { DirectId = x.DirectId, UserId = userId }))
                .OrderByDescending(x => x.DirectTitle)
                .Skip(skip)
                .Take(take)
                .Select(x => _mapper.Map<Direct, DirectModel>(x))
                .ToListAsync();


            for (int i = 0; i < directs.Count; i++)
            {
                var member = await _context.DirectMembers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.DirectId == directs[i].DirectId && x.UserId != userId);
                if(member != null)
                {
                    var avatar = await _context.Avatars
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.UserId == member.UserId);
                    if(avatar != null)
                    {
                        var res = _mapper.Map<Avatar, DirectImageModel>(avatar);
                        directs[i].DirectImage = res;
                    }
                }
            }

            return directs;
        }

        /// <summary>
        /// Создает директ (один на один) с указанным пользователем
        /// </summary>
        public async Task CreateDirectWithUser(Guid currentUserId, CreateDirectModel model)
        {
            if (await CheckUserExistById(currentUserId) == false || await CheckUserExistById(model.UserId) == false)
            {
                throw new UserNotFoundException();
            }

            if((await _context.Directs
                .AsNoTracking()
                .Include(x => x.DirectMembers)
                .FirstOrDefaultAsync(x => x.DirectMembers.Count == 2
                    && x.DirectMembers.Contains(new DirectMembers { UserId = currentUserId, DirectId = x.DirectId })
                    && x.DirectMembers.Contains(new DirectMembers { UserId = model.UserId, DirectId = x.DirectId })))
            != null)
            {
                throw new DirectExistsForbiddenException();
            }

            
            var directId = model.Id ?? Guid.NewGuid();
            var userAvatar = await _context.Avatars
                .AsNoTracking()
                .Include(x => x.Author)
                .FirstOrDefaultAsync(x => x.UserId == model.UserId);

            DirectImages? directImage;
            if (userAvatar != null)
            {
                var attach = await _context.Attaches.AddAsync(new Attach
                {
                    Author = await _context.Users.FirstAsync(x => x.Id == currentUserId),
                    FilePath = userAvatar.FilePath,
                    Id = Guid.NewGuid(),
                    MimeType = userAvatar.MimeType,
                    Name = userAvatar.Name,
                    Size = userAvatar.Size,
                });


                directImage = new DirectImages
                {
                    Author = attach.Entity.Author,
                    DirectId = directId,
                    FilePath = attach.Entity.FilePath,
                    Id = Guid.NewGuid(),
                    MimeType = attach.Entity.MimeType,
                    Name = attach.Entity.Name,
                    Size = attach.Entity.Size,
                };

                await _context.Directs.AddAsync(new Direct
                {
                    DirectId = directId,
                    DirectTitle = model.Title ?? (await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.UserId))!.Name,
                    DirectImage = directImage,
                });
            }
            else
            {
                var direct = await _context.Directs.AddAsync(new Direct
                {
                    DirectId = directId,
                    DirectTitle = model.Title ?? (await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.UserId))!.Name,
                });
            }

            _context.DirectMembers.Add(new DirectMembers
            {
                DirectId = directId,
                UserId = currentUserId,
            });


            _context.DirectMembers.Add(new DirectMembers
            {
                DirectId = directId,
                UserId = model.UserId,
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
                DirectMessageId = model.DirectMessageId ?? Guid.NewGuid(),
                DirectId = model.DirectId,
                DirectMessage = model.Message,
                SenderId = sender,
                Sended = model.Sended ?? DateTimeOffset.UtcNow,
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
