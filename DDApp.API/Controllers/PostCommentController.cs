using DDApp.API.Models;
using DDApp.API.Services;
using DDApp.Common.Consts;
using DDApp.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DDApp.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Api")]
    public class PostCommentController : ControllerBase
    {
        private PostCommentService _postCommentService;

        public PostCommentController(PostCommentService postCommentService)
        {
            _postCommentService = postCommentService;
        }

        [HttpPost]
        [Authorize]
        public async Task CreatePostComment(CreatePostCommentModel model)
            => await _postCommentService.CreatePostComment(GetCurrentUserGuid(), model);

        [HttpPost]
        [Authorize]
        public async Task ChangePostCommentLikeState(Guid commentId)
            => await _postCommentService.ChangePostCommentLikeState(commentId, GetCurrentUserGuid());
        [HttpGet]
        [Authorize]
        public async Task<List<PostCommentModel>> GetPostComments(Guid postId)
            => await _postCommentService.GetPostCommentsByPostId(postId);


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
