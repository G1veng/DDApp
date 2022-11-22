using DDApp.API.Models;
using DDApp.API.Services;
using DDApp.Common.Consts;
using DDApp.Common.Exceptions;
using DDApp.Common.Exceptions.Authorization;
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
            => await _postCommentService.CreatePostComment(GetCurrentUserId(), model);

        [HttpPost]
        [Authorize]
        public async Task ChangePostCommentLikeState(Guid commentId)
            => await _postCommentService.ChangePostCommentLikeState(commentId, GetCurrentUserId());
        [HttpGet]
        [Authorize]
        public async Task<List<PostCommentModel>?> GetPostComments(Guid postId)
            => await _postCommentService.GetPostCommentsByPostId(postId);

        [HttpDelete]
        [Authorize]
        public async Task DeletePostComment(Guid postCommentId)
            => await _postCommentService.DeletePostComment(postCommentId, GetCurrentUserId());

        private Guid GetCurrentUserId()
        {
            var userIdString = User.Claims.FirstOrDefault(x => x.Type == Claims.Id)?.Value;

            if (!Guid.TryParse(userIdString, out var userId))
            {
                throw new UserAuthorizationException();
            }
            else
            {
                return userId;
            }
        }
    }
}
