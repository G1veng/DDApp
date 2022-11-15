using AutoMapper;
using AutoMapper.QueryableExtensions;
using DDApp.API.Models;
using DDApp.API.Models.Post;
using DDApp.Common.Exceptions;
using DDApp.DAL;
using DDApp.DAL.Entites;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DDApp.API.Services
{
    public class PostService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly AttachService _attachmentsService;
        private Func<PostFiles, string?>? _postLinkGenerator;
        private Func<Avatar?, string?>? _userAvatarLinkGenerator;

        public void SetLinkGenerator(Func<PostFiles, string?> postLinkGenerator,
            Func<Avatar?, string?>? userAvatarLinkGenerator)
        {
            _postLinkGenerator = postLinkGenerator;
            _userAvatarLinkGenerator = userAvatarLinkGenerator;
        }

        public PostService(DataContext context, IMapper mapper, AttachService attachmentsService,
            UserService userService, IServer server)
        {
            _context = context;
            _mapper = mapper;
            _attachmentsService = attachmentsService;
        }

        /// <summary>
        /// Добавляет один лайк к комментарию.
        /// Пока что можно добавлять много лайков от одного пользователя,
        /// далее планируется ввести ограничение как связь один к одному,
        /// и рассматривается также вариант, что будут только лайки,
        /// как разность между лайками и дизлайками.
        /// </summary>
        public async Task LikePostComment(Guid commentId)
        {
            var comment = await _context.PostComments.FirstOrDefaultAsync(x => x.Id == commentId);
            if (comment == null || comment.IsActive == false)
            {
                throw new NullArgumentException("Comment not found");
            }

            comment.Likes++;

            await _context.SaveChangesAsync();
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
                .Include(x => x.Author)
                .Include(x => x.Author.Avatar)
                .Include(x => x.PostFiles)
                .Include(x => x.Comments)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (post == null || post.IsActive == false)
            {
                throw new NullArgumentException("Post not found");
            }

            var postModel = _mapper.Map<RequestPostModel>(post);
            postModel.LinkGenerator = _postLinkGenerator;

            var resPost = _mapper.Map<PostModel>(postModel);
            resPost.AuthorAvatar = _userAvatarLinkGenerator == null ? null : _userAvatarLinkGenerator(post.Author.Avatar);

            if(postModel.PostFiles != null)
            {
                postModel.PostFiles.ForEach(file =>
                {
                    resPost.Files?.Add(postModel.LinkGenerator == null ? null : postModel.LinkGenerator(file));
                });
            }

            return resPost;
        }

        /// <summary>
        /// Создает пост и прикрепляет к нему польщователя и изображения, последние.
        /// если имеются
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
                .ToListAsync();

            var postModels = new List<PostModel>();

            posts.ForEach(x => 
            {
                var post = _mapper.Map<Models.Post.RequestPostModel>(x);
                post.LinkGenerator = _postLinkGenerator;

                postModels.Add(_mapper.Map<PostModel>(post));
                postModels[postModels.Count - 1].AuthorAvatar = _userAvatarLinkGenerator == null ? null : _userAvatarLinkGenerator(_context.Avatars.AsNoTracking().Where(a => a.UserId == post.AuthorId).FirstOrDefault());

                if (post.PostFiles != null)
                {
                    post.PostFiles.ForEach(postFile =>
                    {
                        postModels[postModels.Count - 1].Files?.Add(post.LinkGenerator == null ? null : post.LinkGenerator(postFile));
                    });
                }
            });
            return postModels;
        }

        /// <summary>
        /// Возвращает аттач по пути к файлу
        /// </summary>
        public async Task<Attach> GetAttachByAttachId(Guid id)
        {
            var attach = await _context.Attaches.FirstOrDefaultAsync(x => x.Id == id);
            if(attach == null)
            {
                throw new Common.Exceptions.FileException("Attach not found");
            }

            return attach;
        }

        

        /// <summary>
        /// Убирает лайк с комментария
        /// </summary>
        public async Task RemoveLikeFromPostComment(Guid commentId)
        {
            var comment = await _context.PostComments.FirstOrDefaultAsync(x => x.Id == commentId);
            if (comment == null || comment.IsActive == false)
            {
                throw new NullArgumentException("Comment not found");
            }

            comment.Likes--;

            await _context.SaveChangesAsync();
        }
    }
}
