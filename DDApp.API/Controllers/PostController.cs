using DDApp.API.Models;
using DDApp.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DDApp.API.Services;

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
        }

        [HttpPost]
        [Authorize]
        public async Task CreatePost(CreatePostModel model)
        {
            var userIdString = User.Claims.FirstOrDefault(x => x.Type == "id")?.Value;

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
            var userIdString = User.Claims.FirstOrDefault(x => x.Type == "id")?.Value;

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

        [HttpGet]
        public async Task<PostModel> GetPost(Guid postId)
            => await _postService.GetPost(postId);

        [HttpGet]
        public async Task<List<PostCommentModel>> GetPostComments(Guid postId)
            => await _postService.GetPostCommentsByPostId(postId);
    
        [HttpGet]
        public async Task<List<PostModel>> GetPosts()
            => await _postService.GetPosts();

        [HttpGet]
        public async Task<FileResult> GetPictureFromPostByFilePath(string filePath)
        {
            var attach = await _postService.GetImageAttachByFilePath(filePath);

            return File(System.IO.File.ReadAllBytes(attach.FilePath), attach.MimeType);
        }

        [HttpGet]
        public async Task<FileContentResult> DownloadFileByFilePath(string filePath)
            => await _postService.GetAttachByFilePath(filePath);
    }
}
