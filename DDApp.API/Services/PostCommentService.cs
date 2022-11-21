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
        public async Task<List<PostCommentModel>> GetPostCommentsByPostId(Guid postId)
        {
            var comments = await _context.PostComments
                .AsNoTracking()
                .Where(x => x.Post.Id == postId)
                .OrderByDescending(x => x.Created)
                .ProjectTo<PostCommentModel>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return comments;
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
                Text = model.Text,
                Created = DateTimeOffset.UtcNow,
                Author = user,
                Post = post,
            });

            await _context.SaveChangesAsync();
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
    }
}
