using AutoMapper;
using DDApp.API.Models.Direct;
using DDApp.DAL.Entites;

namespace DDApp.API.Mapper.MapperActions
{
    public class DirectModelImageMapperAction : IMappingAction<Direct, DirectModel>
    {
        public void Process(Direct source, DirectModel destination, ResolutionContext context)
        {
            if (source.IsDirectGroup == false)
            {
                var userAvatar = source.DirectMembers?.LastOrDefault()?.User.Avatar;

                if(userAvatar == null)
                {
                    source.DirectImage = null;
                    
                    return;
                }

                source.DirectImage = new DAL.Entites.DirectDir.DirectImages 
                {
                    DirectId = source.DirectId,
                    Id = userAvatar.Id,
                    Name = userAvatar.Name,
                    MimeType = userAvatar.MimeType,
                    FilePath = userAvatar.FilePath,
                    Size = userAvatar.Size,
                };
            }
        }
    }
}
