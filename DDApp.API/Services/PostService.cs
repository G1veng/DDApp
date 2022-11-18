using AutoMapper;
using AutoMapper.QueryableExtensions;
using DDApp.API.Models;
using DDApp.Common.Exceptions;
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
        /// Возвращает все комментарии к определенному посту.
        /// </summary>
        public async Task<List<PostCommentModel>> GetPostCommentsByPostId(Guid postId)
        {
            var comments = await _context.PostComments
                .AsNoTracking()
                .Where(x => x.Post.Id == postId)
                .ProjectTo<PostCommentModel>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return comments;
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
                throw new NullArgumentException("Post not found");
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
                throw new UserException("User not found");
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
        /// Создает пост привязанный к определенному пользователю.
        /// </summary>
        public async Task CreatePostComment(Guid userId, CreatePostCommentModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null || user.IsActive == false)
            {
                throw new UserException("User not found");
            }

            var post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == model.PostId);
            if (post == null || post.IsActive == false)
            {
                throw new NullArgumentException("Post not found");
            }

            var comment = await _context.PostComments.AddAsync(new PostComments
            {
                Text = model.Text,
                Created = DateTimeOffset.UtcNow,
                Author = user,
                Post = post,
            });

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Возвращает все посты.
        /// </summary>
        public async Task<List<PostModel>> GetPosts() {
            var posts = await _context.Posts.AsNoTracking()
                .Where(x => x.IsActive)
                .Include(x => x.Author)
                .Include(x => x.PostFiles)
                .Include(x => x.Comments)
                .Include(x => x.PostLikes)
                .Select(x => _mapper.Map<Posts, PostModel>(x))
                .ToListAsync();

            if(posts == null || posts == default)
            {
                throw new Exception("Posts not found"); //Надо ли????
            }

            return posts;
        }

        /// <summary>
        /// Возвращает аттач по пути к файлу.
        /// </summary>
        public async Task<Attach> GetAttachByAttachId(Guid id)
        {
            var attach = await _context.Attaches.FirstOrDefaultAsync(x => x.Id == id);
            if(attach == null)
            {
                throw new FileException("Attach not found");
            }

            return attach;
        }

        /// <summary>
        /// Если лайк был он удаляется, если его не было, то он доавляется.
        /// </summary>
        public async Task ChangePostCommentLikeState(Guid postCommentId, Guid userId)
        {
            var like = new PostCommentLikes
            {
                UserId = userId,
                PostCommentId = postCommentId,
            };

            if (_context.PostCommentsLikes.Contains(like))
            {
                _context.PostCommentsLikes.Remove(like);
            }
            else
            {
                _context.PostCommentsLikes.Add(like);
            }

            await _context.SaveChangesAsync();
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

        /// <summary>
        /// Добавляет лайк к посту.
        /// </summary>
        public async Task AddPostLike(Guid postId, Guid userId)
        {
            var postLike = new PostLikes
            {
                PostId = postId,
                UserId = userId,
            };

            if (_context.PostLikes.Contains(postLike))
            {
                throw new Exception("Cannot like more than one time");
            }

            _context.PostLikes.Add(postLike);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Убирает лайк с поста.
        /// </summary>
        public async Task RemoveLikePost(Guid postId, Guid userId)
        {
            var postLike = new PostLikes
            {
                PostId = postId,
                UserId = userId,
            };

            if (!_context.PostLikes.Contains(postLike))
            {
                throw new Exception("Cannot like more than one time");
            }

            _context.PostLikes.Remove(postLike);

            await _context.SaveChangesAsync();
        }
    }
}
