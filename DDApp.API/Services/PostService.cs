using AutoMapper;
using AutoMapper.QueryableExtensions;
using DDApp.API.Models;
using DDApp.Common.Exceptions;
using DDApp.Common.Exceptions.NotFound;
using DDApp.DAL;
using DDApp.DAL.Entites;
using Microsoft.EntityFrameworkCore;
using DDApp.API.Models.Subscription;
using DDApp.Common.Exceptions.Authorization;
using FileNotFoundException = DDApp.Common.Exceptions.NotFound.FileNotFoundException;

namespace DDApp.API.Services
{
    public class PostService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly AttachService _attachmentsService;

        public PostService(DataContext context, IMapper mapper, AttachService attachmentsService)
        {
            _context = context;
            _mapper = mapper;
            _attachmentsService = attachmentsService;
        }

        /// <summary>
        /// Возвращает по идентефикатору.
        /// </summary>
        public async Task<PostModel> GetPost(Guid id)
        {
            var post = await _context.Posts
                .AsNoTracking()
                .Include(x => x.Author).ThenInclude(x => x.Avatar)
                .Include(x => x.PostFiles)
                .Include(x => x.Comments)
                .Include(x => x.PostLikes)
                .Where(x => x.IsActive)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (post == null || post.IsActive == false)
            {
                throw new PostNotFoundException();
            }

            return _mapper.Map<PostModel>(post);
        }

        /// <summary>
        /// Создает пост и прикрепляет к нему польщователя и изображения, последние
        /// если имеются.
        /// </summary>
        public async Task CreatePost(Guid userId, CreatePostModel model)
        {
            var user = await _context.Users.Include(x => x.Posts).FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null || user.IsActive == false)
            {
                throw new UserNotFoundException();
            }

            if(model.Files == null || model.Files.Count == 0)
            {
                throw new FileNotFoundException();
            }
            else
            {
                var filePath = string.Empty;
                var post = await _context.Posts.AddAsync(new Posts
                {
                    Id = model.Id ?? new Guid(),
                    Author = user,
                    Text = model.Text,
                    Created = model.Created ?? DateTimeOffset.UtcNow,
                }) ;

                if (model.Files != null)
                {
                    foreach (var meta in model.Files)
                    {
                        filePath = _attachmentsService.CopyImageVideoFile(meta);

                        var postFile = await _context.PostFiles.AddAsync(new PostFiles
                        {
                            Id = meta.TempId ?? new Guid(),
                            PostId = post.Entity.Id,
                            Name = meta.Name,
                            Author = user,
                            MimeType = meta.MimeType,
                            Size = meta.Size,
                            FilePath = filePath,
                        });
                    }
                }

                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Удаляет выбранный пост
        /// </summary>
        public async Task DeletePost(Guid postId, Guid userId)
        {
            var post = await _context.Posts.Include(x => x.Author).FirstOrDefaultAsync(x => x.Id == postId);

            if(post == default || post == null || post.IsActive == false)
            {
                throw new PostNotFoundException();
            }

            if(post.Author.Id != userId)
            {
                throw new UserAuthorizationException();
            }

            post.IsActive = false;

            await _context.SaveChangesAsync();
        }

        public async Task<int> GetUserPostAmount(Guid userId)
            =>  await _context.Posts.CountAsync(x => x.Author.Id == userId && x.IsActive == true);

        /// <summary>
        /// Возвращает посты, созданные подписками данного пользователя
        /// </summary>
        public async Task<List<PostModel>?> GetSubscriptionsPosts(Guid userId, int skip, int take, DateTimeOffset? lastPostCreated)
        {
            var subscriptions = await _context.Subscriptions
                .AsNoTracking()
                .Where(x => x.SubscriberId == userId && x.UserSubscription.IsActive)
                .Select(x => _mapper.Map<Subscriptions, OnlySubscriptionModel>(x))
                .ToListAsync();

            List<Guid> subscriptionsGuids = new List<Guid>();
            foreach(var sub in subscriptions)
            {
                subscriptionsGuids.Add(sub.SubscriptionId);
            }

            var posts = await _context.Posts
                .AsNoTracking()
                .Where(x => x.IsActive && subscriptionsGuids.Contains(x.Author.Id) && (lastPostCreated == null ? true : x.Created < lastPostCreated))
                .OrderByDescending(x => x.Created)
                .Skip(skip)
                .Take(take)
                .Include(x => x.PostFiles)
                .Include(x => x.Comments)
                .Include(x => x.PostLikes)
                .Include(x => x.Author)
                .Include(x => x.Author).ThenInclude(x => x.Avatar)
                .Select(x => _mapper.Map<Posts, PostModel>(x))
                .ToListAsync();

            return posts;
        }

        /// <summary>
        /// Возвращает посты текущего пользвателя.
        /// </summary>
        public async Task<List<PostModel>> GetCurrentUserPosts(DateTimeOffset? lastPostCreated, int skip, int take, Guid userId)
        {
            var posts = await _context.Posts
                .AsNoTracking()
                .Where(x => x.IsActive && x.Author.Id == userId && (lastPostCreated == null ? true : x.Created > lastPostCreated))
                .OrderByDescending(x => x.Created)
                .Skip(skip)
                .Take(take)
                .Include(x => x.PostFiles)
                .Include(x => x.Comments)
                .Include(x => x.PostLikes)
                .Include(x => x.Author)
                .Include(x => x.Author).ThenInclude(x => x.Avatar)
                .Select(x => _mapper.Map<Posts, PostModel>(x))
                .ToListAsync();

            if (posts == null || posts == default)
            {
                throw new PostNotFoundException();
            }

            return posts;
        }

        /// <summary>
        /// Возвращает все посты.
        /// </summary>
        public async Task<List<PostModel>> GetPosts(DateTimeOffset? lastPostCreated, int skip, int take, Guid userId) {
            var user = await _context.Users.AsNoTracking().Include(x => x.Posts).FirstOrDefaultAsync(x => x.Id == userId);
            if(user == default || user == null || user.IsActive == false)
            {
                throw new UserNotFoundException();
            }

            var posts = await _context.Posts
                .AsNoTracking()
                .Where(x => x.IsActive && x.Author.Id == userId && (lastPostCreated == null ? true : x.Created > lastPostCreated))
                .OrderByDescending(x => x.Created)
                .Skip(skip)
                .Take(take)
                .Include(x => x.PostFiles)
                .Include(x => x.Comments)
                .Include(x => x.PostLikes)
                .Include(x => x.Author)
                .Include(x => x.Author).ThenInclude(x => x.Avatar)
                .Select(x => _mapper.Map<Posts, PostModel>(x))
                .ToListAsync();

            if(posts == null || posts == default)
            {
                throw new PostNotFoundException();
            }

            return posts;
        }

        /// <summary>
        /// Если лайк на посте был, то он удаляется,
        /// в противном случае добавляется.
        /// </summary>
        public async Task ChangePostLikeState(Guid postId, Guid userId)
        {
            var like = new PostLikes
            {
                UserId = userId,
                PostId = postId,
            };

            if (_context.PostLikes.Contains(like))
            {
                _context.PostLikes.Remove(like);
            }
            else
            {
                _context.PostLikes.Add(like);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> GetPostLikeState(Guid postId, Guid userId)
        {
            if (await _context.PostLikes.FirstOrDefaultAsync(x => x.PostId == postId && x.UserId == userId) == null)
            {
                return false;
            }
            return true;
        }
    }
}
