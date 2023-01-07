using AutoMapper;
using DDApp.API.Models.Direct;
using DDApp.DAL.Entites;

namespace DDApp.API.Mapper.MapperActions
{
    public class AvatarDirectImageMapperAction : IMappingAction<Avatar, DirectImageModel>
    {
        private readonly Func<Avatar?, string?>? _avatarLinkHelper;

        public AvatarDirectImageMapperAction(Services.LinkGeneratorService linkGeneratorService)
        {
            _avatarLinkHelper = linkGeneratorService.AvatarDirectImageLinkGenerator;
        }

        public void Process(Avatar source, DirectImageModel destination, ResolutionContext context)
        {
            destination.Link = source == null ? null : _avatarLinkHelper == null ? null : _avatarLinkHelper(source);
        }
    }
}
