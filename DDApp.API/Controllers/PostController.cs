using DDApp.API.Models;
using DDApp.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DDApp.API.Services;
using DDApp.Common.Consts;
using DDApp.Common.Extensions;
using DDApp.Common.Exceptions.Authorization;

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
            linkGeneratorService.AvatarLinkGenerator = 
                y => Url.ControllerAction<AttachController>(nameof(AttachController.GetUserAvatarByAttachId), new { attachId = y?.Id });
            linkGeneratorService.PostAuthorAvatarLinkGenerator = 
                z => Url.ControllerAction<AttachController>(nameof(AttachController.GetUserAvatarByUserId), new { userId = z });
            linkGeneratorService.PostFileLinkGenerator = 
                x => Url.ControllerAction<AttachController>(nameof(AttachController.GetPostPictureByAttchId), new { postContentId = x?.Id });
        }

        [HttpPost]
        [Authorize]
        public async Task CreatePost(CreatePostModel model)
            => await _postService.CreatePost(GetCurrentUserGuid(), model);

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
        public async Task<List<PostModel>> GetPosts(int skip = 0, int take = 10)
            => await _postService.GetPosts(skip, take);


        private Guid GetCurrentUserGuid()
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
