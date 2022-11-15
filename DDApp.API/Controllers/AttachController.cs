using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DDApp.API.Models;
using DDApp.API.Services;
using Microsoft.AspNetCore.Authorization;
using DDApp.DAL.Entites;

namespace DDApp.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AttachController : ControllerBase
    {
        private readonly AttachService _attachmentsService;
        private readonly UserService _userService;
        private readonly PostService _postService;

        public AttachController(AttachService attachmentsService, UserService userService,
            PostService postService)
        {
            _attachmentsService = attachmentsService;
            _userService = userService;
            _postService = postService;
            _userService.SetLinkGenerator(x => Url.Action(nameof(GetUserAvatarByAttachId), new { userId = x?.UserId}));
            /*_postService.SetLinkGenerator(
                x => Url.Action(nameof(PostController.GetPostPicture), new { id = x.Id }),
                y => Url.Action(nameof(GetUserAvatar), new { y?.Id }));*/
        }

        

        [HttpPost]
        [DisableRequestSizeLimit]
        [Authorize]
        public async Task<List<MetadataModel>> UploadFiles([FromForm] List<IFormFile> files)
        {
            if(files.Count == 0)
            {
                throw new Common.Exceptions.FileException("Files not found");
            }

            return await _attachmentsService.UploadFiles(files);
        }

        [HttpGet]
        [Route("{userId}")]
        [AllowAnonymous]
        public async Task<FileStreamResult> GetUserAvatarByAttachId(Guid userId, bool download)
            => GetFile(await _attachmentsService.GetImageAttachByAttachId(userId), download);

        [HttpGet]
        [Route("{postContentId}")]
        [AllowAnonymous]
        public async Task<FileStreamResult> GetPostPictureByAttchId(Guid postContentId, bool download)
            => GetFile(await _attachmentsService.GetImageAttachByAttachId(postContentId), download);


        private FileStreamResult GetFile(Attach attach, bool download = false)
        {
            var fs = new FileStream(attach.FilePath, FileMode.Open);

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
