using AutoMapper;
using AutoMapper.QueryableExtensions;
using DDApp.API.Models;
using DDApp.Common.Exceptions;
using DDApp.DAL;
using DDApp.DAL.Entites;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DDApp.API.Services
{
    public class PostService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly AttachmentsService _attachmentsService;
        private readonly UserService _userService;

        public PostService(DataContext context, IMapper mapper, AttachmentsService attachmentsService,
            UserService userService)
        {
            _context = context;
            _mapper = mapper;
            _attachmentsService = attachmentsService;
            _userService = userService;
        }

        /// <summary>
        /// Добавляет один лайк к комментарию
        /// </summary>
        public async Task LikePostComment(Guid commentId)
        {
            var comment = await _context.PostComments.FirstOrDefaultAsync(x => x.Id == commentId);
            if (comment == null)
            {
                throw new NullArgumentException("Comment not found");
            }

            comment.Likes++;

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Возвращает все комментарии к определенному посту
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
        /// Возвращает по идентефикатору
        /// </summary>
        public async Task<PostModel> GetPost(Guid id)
        {
            var post = await _context.Posts
                .Include(x => x.Author)
                .Include(x => x.PostFiles)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (post == null)
            {
                throw new NullArgumentException("Post not found");
            }

            var postModel = _mapper.Map<PostModel>(post);

            if (post.PostFiles != null)
            {
                if (postModel.Files == null)
                {
                    postModel.Files = new List<string>();
                }

                foreach (var file in post.PostFiles)
                {
                    postModel.Files.Add(file.FilePath);
                }
            }

            return postModel;
        }

        /// <summary>
        /// Создает пост и прикрепляет к нему польщователя и изображения, последние
        /// если имеются
        /// </summary>
        public async Task CreatePost(Guid userId, CreatePostModel model)
        {
            var user = await _context.Users.Include(x => x.Posts).FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                throw new UserNotFoundException("User not found");
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
        /// Создает пост привязанный к определенному пользователю
        /// </summary>
        public async Task CreatePostComment(Guid userId, CreatePostCommentModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
            {
                throw new UserNotFoundException("User not found");
            }

            var post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == model.PostId);
            if (post == null)
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
        /// Возвращает все посты
        /// </summary>
        public async Task<List<PostModel>> GetPosts() {
            var posts = await _context.Posts.AsNoTracking()
                .Include(x => x.Author)
                .Include(x => x.PostFiles)
                .ToListAsync();

            var postModels = new List<PostModel>();

            if (posts != null)
            {
                foreach (var post in posts)
                {
                    postModels.Add(_mapper.Map<PostModel>(post));

                    if (postModels[postModels.Count - 1].Files == null)
                    {
                        postModels[postModels.Count - 1].Files = new List<string>();
                    }

                    if (post.PostFiles != null)
                    {
                        foreach (var file in post.PostFiles)
                        {
                            postModels[postModels.Count - 1].Files?.Add(file.FilePath);
                        }
                    }
                }
            }

            return postModels;
        }

        /// <summary>
        /// Возвращает аттач по пути к файлу
        /// </summary>
        public async Task<FileContentResult> GetAttachByFilePath(string filePath)
        {
            var attach = await _context.Attaches.FirstOrDefaultAsync(x => x.FilePath == filePath.Replace("\\\\", "\\"));
            if(attach == null)
            {
                throw new Common.Exceptions.FileNotFoundException("Attach not found");
            }

            var fileBytes = await File.ReadAllBytesAsync(attach.FilePath);

            return new FileContentResult(fileBytes, attach.MimeType)
            {
                FileDownloadName = attach.Name
            };
        }

        /// <summary>
        /// Возвращает аттач для переданного пути
        /// </summary>
        public async Task<Attach> GetImageAttachByFilePath(string filePath)
        {
            var attach = await _context.Attaches.FirstOrDefaultAsync(x => x.FilePath == filePath.Replace("\\\\", "\\"));
            if (attach == null)
            {
                throw new Common.Exceptions.FileNotFoundException("Attach not found");
            }

            if (!Common.MimeTypeHelper.CheckImageMimeType(System.IO.File.ReadAllBytes(attach.FilePath)))
            {
                throw new WrongTypeException("File is not image format");
            }

            return attach;
        }

        /// <summary>
        /// Убирает лайк с комментария
        /// </summary>
        public async Task RemoveLikeFromPostComment(Guid commentId)
        {
            var comment = await _context.PostComments.FirstOrDefaultAsync(x => x.Id == commentId);
            if (comment == null)
            {
                throw new NullArgumentException("Comment not found");
            }

            if (comment.Likes > 0)
            {
                comment.Likes--;
            }

            await _context.SaveChangesAsync();
        }
    }
}
