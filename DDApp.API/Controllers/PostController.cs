using DDApp.API.Models;
using DDApp.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DDApp.API.Services;
using DDApp.Common.Consts;
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
            => await _postService.CreatePost(GetCurrentUserGuid(), model);

        [HttpPost]
        [Authorize]
        public async Task CreatePostComment(CreatePostCommentModel model)
            => await _postService.CreatePostComment(GetCurrentUserGuid(), model);

        [HttpPost]
        [Authorize]
        public async Task ChangePostCommentLikeState(Guid commentId)
            => await _postService.ChangePostCommentLikeState(commentId, GetCurrentUserGuid());

        [HttpPost]
        [Authorize]
        public async Task ChangePostLikeState(Guid postId)
            => await _postService.ChangePostLikeState(postId, GetCurrentUserGuid());

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


        private Guid GetCurrentUserGuid()
        {
            var userIdString = User.Claims.FirstOrDefault(x => x.Type == Claims.Id)?.Value;

            if (!Guid.TryParse(userIdString, out var userId))
            {
                throw new AuthorizationException("You are not authorized");
            }
            else
            {
                return userId;
            }
        } 
    }
}
