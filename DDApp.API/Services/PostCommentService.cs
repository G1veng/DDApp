using AutoMapper;
using AutoMapper.QueryableExtensions;
using DDApp.API.Models;
using DDApp.Common.Exceptions;
using DDApp.Common.Exceptions.NotFound;
using DDApp.DAL;
using DDApp.DAL.Entites;
using Microsoft.EntityFrameworkCore;
using DDApp.Common.Exceptions.Authorization;

namespace DDApp.API.Services
{
    public class PostCommentService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly AttachService _attachmentsService;

        public PostCommentService(DataContext context, IMapper mapper, AttachService attachmentsService)
        {
            _context = context;
            _mapper = mapper;
            _attachmentsService = attachmentsService;
        }

        /// <summary>
        /// Возвращает все комментарии к определенному посту.
        /// </summary>
        public async Task<List<PostCommentModel>?> GetPostCommentsByPostId(DateTimeOffset? lastPostCreated, Guid postId, int take, int skip)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == postId);

            if (post == null || post == default)
            {
                throw new PostNotFoundException();
            }

            return await _context.PostComments
                .AsNoTracking()
                .Where(x => x.Post.Id == postId && (lastPostCreated == null ? true : x.Created < lastPostCreated))
                .OrderByDescending(x => x.Created)
                .Skip(skip)
                .Take(take)
                .ProjectTo<PostCommentModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        /// <summary>
        /// Создает пост привязанный к определенному пользователю.
        /// </summary>
        public async Task CreatePostComment(Guid userId, CreatePostCommentModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null || user.IsActive == false)
            {
                throw new UserNotFoundException();
            }

            var post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == model.PostId);
            if (post == null || post.IsActive == false)
            {
                throw new PostNotFoundException();
            }

            var comment = await _context.PostComments.AddAsync(new PostComments
            {
                Id = model.Id,
                Text = model.Text,
                Created = model.Created,
                Author = user,
                Post = post,
            });

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Удаляет комментарий к посту по его id
        /// </summary>
        public async Task DeletePostComment(Guid postCommentId, Guid userId)
        {
            var postComment = await _context.PostComments
                .Include(x => x.Author)
                .Include(x => x.Post)
                .ThenInclude(x => x.Author)
                .FirstOrDefaultAsync(x => x.Id == postCommentId);

            if(postComment == null || postComment == default || !postComment.IsActive)
            {
                throw new PostCommentNotFoundException();
            }

            if (!postComment.Post.IsActive)
            {
                throw new PostNotFoundException();
            }

            if(postComment.Author.Id != userId || postComment.Post.Author.Id != userId)
            {
                throw new UserAuthorizationException();
            }

            postComment.IsActive = false;

            await _context.SaveChangesAsync();
        }

        
        /// <summary>
        /// Если лайк был он удаляется, если его не было, то он доавляется.
        /// </summary>
        public async Task ChangePostCommentLikeState(Guid postCommentId, Guid userId)
        {
            if((await _context.PostComments.FirstOrDefaultAsync(x => x.Id == postCommentId)) == null)
            {
                throw new PostCommentNotFoundException();
            }

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
    }
}
