using AutoMapper;
using AutoMapper.QueryableExtensions;
using DDApp.API.Models;
using DDApp.Common.Exceptions;
using DDApp.Common.Exceptions.NotFound;
using DDApp.DAL;
using DDApp.DAL.Entites;
using Microsoft.EntityFrameworkCore;

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
            else
            {
                var filePath = string.Empty;
                var post = await _context.Posts.AddAsync(new Posts
                {
                    Author = user,
                    Created = DateTimeOffset.UtcNow,
                    Text = model.Text,
                });

                if (model.Files != null)
                {
                    foreach (var meta in model.Files)
                    {
                        filePath = _attachmentsService.CopyImageVideoFile(meta);

                        var postFile = await _context.PostFiles.AddAsync(new PostFiles
                        {
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
        /// Возвращает все посты.
        /// </summary>
        public async Task<List<PostModel>> GetPosts(int skip, int take) {
            var posts = await _context.Posts.AsNoTracking()
                .Where(x => x.IsActive)
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
                throw new Exception("Posts not found"); //Надо ли????
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
    }
}
