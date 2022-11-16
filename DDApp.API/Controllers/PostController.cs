using DDApp.API.Models;
using DDApp.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DDApp.API.Services;
using DDApp.Common.Consts;
using DDApp.DAL.Entites;
using DDApp.Common.Extensions;

namespace DDApp.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Api")]
    public class PostController : ControllerBase
    {
        private readonly PostService _postService;
        private readonly AttachService _attachService;

        public PostController(PostService postService, AttachService attachService,
            LinkGeneratorService linkGeneratorService)
        {
            _attachService = attachService;
            _postService = postService;
            linkGeneratorService.SetLinkGenerators(
                x => Url.ControllerAction<AttachController>(nameof(AttachController.GetPostPictureByAttchId), new { postContentId = x?.Id}),
                y => Url.ControllerAction<AttachController>(nameof(AttachController.GetUserAvatarByAttachId), new { attachId = y?.Id }),
                z => Url.ControllerAction<AttachController>(nameof(AttachController.GetUserAvatarByUserId), new { userId = z }));
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
        [AllowAnonymous]
        public async Task<PostModel> GetPost(Guid postId)
            => await _postService.GetPost(postId);

        [HttpGet]
        [Authorize]
        public async Task<List<PostCommentModel>> GetPostComments(Guid postId)
            => await _postService.GetPostCommentsByPostId(postId);
    
        [HttpGet]
        [AllowAnonymous]
        public async Task<List<PostModel>> GetPosts()
            => await _postService.GetPosts();
    }
}
