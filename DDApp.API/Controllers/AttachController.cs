using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DDApp.API.Models;
using DDApp.API.Services;

namespace DDApp.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AttachController : ControllerBase
    {
        private readonly AttachmentsService _attachmentsService;

        public AttachController(AttachmentsService attachmentsService)
        {
            _attachmentsService = attachmentsService;
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<List<MetadataModel>> UploadFiles([FromForm] List<IFormFile> files)
        {
            if(files.Count == 0)
            {
                throw new Common.Exceptions.FileException("Files not found");
            }

            return await _attachmentsService.UploadFiles(files);
        }
    }
}
