using DDApp.API.Models;
using DDApp.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DDApp.API.Services;
using DDApp.Common.Consts;

namespace DDApp.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly PostService _postService;

        public PostController(PostService postService)
        {
            _postService = postService;
            _postService.SetLinkGenerator(x => 
                Url.Action(nameof(GetPictureFromPostByAttachId),  new { id = x.Id, download = false }));
        }

        [HttpPost]
        [Authorize]
        public async Task CreatePost(CreatePostModel model)
        {
            var userIdString = User.Claims.FirstOrDefault(x => x.Type == Claims.Id)?.Value;

            if (!Guid.TryParse(userIdString, out var userId))
            {
                throw new AuthorizationException("You are not authorized");
            }
            else
            {
                await _postService.CreatePost(userId, model);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task CreatePostComment(CreatePostCommentModel model)
        {
            var userIdString = User.Claims.FirstOrDefault(x => x.Type == Claims.Id)?.Value;

            if (!Guid.TryParse(userIdString, out var userId))
            {
                throw new AuthorizationException("You are not authorized");
            }
            else
            {
                await _postService.CreatePostComment(userId, model);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task LikePostComment(Guid commentId)
            => await _postService.LikePostComment(commentId);

        [HttpPost]
        [Authorize]
        public async Task RemoveLikeFromPostComment(Guid commentId)
            => await _postService.RemoveLikeFromPostComment(commentId);

        [HttpGet]
        [Authorize]
        public async Task<PostModel> GetPost(Guid postId)
            => await _postService.GetPost(postId);

        [HttpGet]
        [Authorize]
        public async Task<List<PostCommentModel>> GetPostComments(Guid postId)
            => await _postService.GetPostCommentsByPostId(postId);
    
        [HttpGet]
        [Authorize]
        public async Task<List<PostModel>> GetPosts()
            => await _postService.GetPosts();

        [HttpGet]
        [AllowAnonymous]
        public async Task<FileStreamResult> GetPictureFromPostByAttachId(Guid id, bool download)
        {
            var attach = await _postService.GetImageAttachByFilePath(id);

            FileStream fs = new FileStream(attach.FilePath, FileMode.OpenOrCreate);

            if (download)
            {
                return File(fs, attach.MimeType, attach.Name);
            }
            else
            {
                return File(fs, attach.MimeType);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<FileStreamResult> DownloadFileByFileId(Guid id, bool download) 
        {
            var attach = await _postService.GetAttachByAttachId(id);

            FileStream fs = new FileStream(attach.FilePath, FileMode.OpenOrCreate);

            if (download)
            {
                return File(fs, attach.MimeType, attach.Name);
            }
            else
            {
                return File(fs, attach.MimeType);
            }
        }
    }
}
