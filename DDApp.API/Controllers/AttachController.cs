using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DDApp.API.Models;

namespace DDApp.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AttachController : ControllerBase
    {
        [HttpPost]
        public async Task<List<MetadataModel>> UploadFiles([FromForm] List<IFormFile> files)
        {
            var res = new List<MetadataModel>();

            foreach(var file in files)
            {
                res.Add(await UploadFile(file));
            }

            return res;
        }

        
        private async Task<MetadataModel> UploadFile(IFormFile file)
         {
            var tempPath = Path.GetTempPath();
            var meta = new MetadataModel
            {
                TempId = Guid.NewGuid(),
                Name = file.FileName,
                MimeType = file.ContentType,
                Size = file.Length,
            };

            var newPath = Path.Combine(tempPath, meta.TempId.ToString());

            var fileInfo = new FileInfo(newPath);
            if (fileInfo.Exists)
            {
                throw new Exception("File exists");
            }
            else
            {
                if (fileInfo.Directory == null)
                {
                    throw new Exception("Temp is null");
                }
                else
                {
                    if (!fileInfo.Directory.Exists)
                    {
                        fileInfo.Directory?.Create();
                    }
                }

                using (var stream = System.IO.File.Create(newPath))
                {
                    await file.CopyToAsync(stream);
                }

                return meta;
            }
        }
    }
}
