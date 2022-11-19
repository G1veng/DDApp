using Microsoft.AspNetCore.Mvc;
using DDApp.API.Models;
using DDApp.API.Services;
using Microsoft.AspNetCore.Authorization;
using DDApp.DAL.Entites;

namespace DDApp.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Api")]
    public class AttachController : ControllerBase
    {
        private readonly AttachService _attachmentsService;

        public AttachController(AttachService attachmentsService)
        {
            _attachmentsService = attachmentsService;
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
        public async Task<FileStreamResult?> GetUserAvatarByUserId(Guid userId, bool download)
            => GetFile(await _attachmentsService.GetUserAvatarByUserId(userId), download);

        [HttpGet]
        [Route("{attachId}")]
        [AllowAnonymous]
        public async Task<FileStreamResult> GetUserAvatarByAttachId(Guid attachId, bool download)
            => GetFile(await _attachmentsService.GetImageAttachByAttachId(attachId), download);

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
