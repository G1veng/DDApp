using DDApp.API.Models;
using DDApp.Common.Exceptions;
using DDApp.DAL.Entites;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DDApp.API.Services
{
    public class AttachService
    {
        private readonly DDApp.DAL.DataContext _context;

        public AttachService(DDApp.DAL.DataContext context)
        {
            _context = context;
        }

        public async Task<List<MetadataModel>> UploadFiles(List<IFormFile> files)
        {
            var res = new List<MetadataModel>();

            foreach (var file in files)
            {
                res.Add(await UploadFile(file));
            }

            return res;
        }

        /// <summary>
        /// Возвращает аттач для переданного пути
        /// </summary>
        public async Task<Attach> GetImageAttachByAttachId(Guid id)
        {
            var attach = await _context.Attaches.FirstOrDefaultAsync(x => x.Id == id);
            if (attach == null)
            {
                throw new Common.Exceptions.FileException("Attach not found");
            }

            if (!Common.MimeTypeHelper.CheckImageMimeType(System.IO.File.ReadAllBytes(attach.FilePath)))
            {
                throw new WrongTypeException("File is not image format");
            }

            return attach;
        }

        public async Task<MetadataModel> UploadFile(IFormFile file)
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
                throw new FileException("File exists");
            }
            else
            {
                if (fileInfo.Directory == null)
                {
                    throw new NullArgumentException("Temp is null");
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

        public string CopyImageFile(MetadataModel model)
        {
            var tempFi = new FileInfo(Path.Combine(Path.GetTempPath(), model.TempId.ToString()));
            if (!tempFi.Exists)
            {
                throw new Common.Exceptions.FileException("File not found");
            }
            if (!Common.MimeTypeHelper.CheckImageMimeType(System.IO.File.ReadAllBytes(tempFi.FullName)))
            {
                throw new WrongTypeException("File is not image");
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), Common.Consts.FileDirectories.Attaches, model.TempId.ToString());

            var destFi = new FileInfo(path);
            if (destFi.Directory != null && !destFi.Directory.Exists)
            {
                destFi.Directory.Create();
            }

            System.IO.File.Copy(tempFi.FullName, path, true);

            tempFi.Delete();

            return path;
        }

        public string CopyImageVideoFile(MetadataModel model)
        {
            var tempFi = new FileInfo(Path.Combine(Path.GetTempPath(), model.TempId.ToString()));
            if (!tempFi.Exists)
            {
                throw new Common.Exceptions.FileException("File not found");
            }
            if (!Common.MimeTypeHelper.CheckImageMimeType(System.IO.File.ReadAllBytes(tempFi.FullName)) &&
                !Common.MimeTypeHelper.CheckVideoMimeTypeByMimeType(model.MimeType))
            {
                throw new WrongTypeException("File is not image or video");
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), Common.Consts.FileDirectories.Attaches, model.TempId.ToString());

            var destFi = new FileInfo(path);
            if (destFi.Directory != null && !destFi.Directory.Exists)
            {
                destFi.Directory.Create();
            }

            System.IO.File.Copy(tempFi.FullName, path, true);

            tempFi.Delete();

            return path;
        }


        
    }
}
