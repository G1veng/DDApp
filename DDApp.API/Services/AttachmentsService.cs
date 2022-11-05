using Microsoft.EntityFrameworkCore;
using DDApp.DAL.Entites;
using DDApp.API.Models;
using AutoMapper;

namespace DDApp.API.Services
{
    public class AttachmentsService
    {
        private readonly DDApp.DAL.DataContext _context;

        public AttachmentsService(DDApp.DAL.DataContext context)
        {
            _context = context;
        }

        public async Task AddAvatarToUser(Guid userId, MetadataModel meta, string filePath)
        {
            var user = await _context.Users.Include(x => x.Avatar).FirstOrDefaultAsync(x => x.Id == userId);

            if (user != null)
            {

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
    }
}
