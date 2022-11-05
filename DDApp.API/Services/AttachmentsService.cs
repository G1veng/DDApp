using Microsoft.EntityFrameworkCore;
using DDApp.DAL.Entites;
using DDApp.API.Models;
using AutoMapper;
using DDApp.Common.Exceptions;

namespace DDApp.API.Services
{
    public class AttachmentsService
    {
        private readonly DDApp.DAL.DataContext _context;

        public AttachmentsService(DDApp.DAL.DataContext context)
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
                throw new FileExistException("File exists");
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

        public async Task AddAvatarToUser(Guid userId, MetadataModel meta)
        {
            var user = await _context.Users.Include(x => x.Avatar).FirstOrDefaultAsync(x => x.Id == userId);

            if (user != null)
            {

                var filePath = CopyImageFile(meta);

                var avatar = new Avatar
                {
                    User = user,
                    Author = user,
                    MimeType = meta.MimeType,
                    FilePath = filePath,
                    Size = meta.Size,
                    Name = meta.Name,
                };
                user.Avatar = avatar;

                await _context.SaveChangesAsync();

            }
        }

        public async Task CreatePost(Guid userId, CreatePostModel model)
        {
            var user = await _context.Users.Include(x => x.Posts).FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                throw new UserNotFoundException("User not found");
            }
            else
            {
                var filePath = string.Empty;
                var post = await _context.Posts.AddAsync(new Posts
                {
                    Author = user,
                    Created = DateTimeOffset.UtcNow,
                    Text = model.Text,
                });

                if (model.Files != null)
                {
                    foreach (var meta in model.Files)
                    {
                        filePath = CopyImageVideoFile(meta);

                        var postFile = await _context.PostFiles.AddAsync( new PostFiles
                        {
                            PostId = post.Entity.Id,
                            Name = meta.Name,
                            Author = user,
                            MimeType = meta.MimeType,
                            Size = meta.Size,
                            FilePath = filePath,
                        });
                    }
                }

                await _context.SaveChangesAsync();
            }
        }

        private string CopyImageFile(MetadataModel model)
        {
            var tempFi = new FileInfo(Path.Combine(Path.GetTempPath(), model.TempId.ToString()));
            if (!tempFi.Exists)
            {
                throw new Common.Exceptions.FileNotFoundException("File not found");
            }
            if (!Common.MimeTypeHelper.CheckImageMimeType(System.IO.File.ReadAllBytes(tempFi.FullName)))
            {
                throw new WrongTypeException("File is not image");
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "attaches", model.TempId.ToString());

            var destFi = new FileInfo(path);
            if (destFi.Directory != null && !destFi.Directory.Exists)
            {
                destFi.Directory.Create();
            }

            System.IO.File.Copy(tempFi.FullName, path, true);

            return path;
        }

        private string CopyImageVideoFile(MetadataModel model)
        {
            var tempFi = new FileInfo(Path.Combine(Path.GetTempPath(), model.TempId.ToString()));
            if (!tempFi.Exists)
            {
                throw new Common.Exceptions.FileNotFoundException("File not found");
            }
            if (!Common.MimeTypeHelper.CheckImageMimeType(System.IO.File.ReadAllBytes(tempFi.FullName)) &&
                !Common.MimeTypeHelper.CheckVideoMimeTypeByMimeType(model.MimeType))
            {
                throw new WrongTypeException("File is not image or video");
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "attaches", model.TempId.ToString());

            var destFi = new FileInfo(path);
            if (destFi.Directory != null && !destFi.Directory.Exists)
            {
                destFi.Directory.Create();
            }

            System.IO.File.Copy(tempFi.FullName, path, true);

            return path;
        }
    }
}
