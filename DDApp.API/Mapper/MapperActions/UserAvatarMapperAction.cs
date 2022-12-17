using AutoMapper;
using DDApp.API.Models;
using DDApp.DAL.Entites;

namespace DDApp.API.Mapper.MapperActions
{
    public class UserAvatarMapperAction : IMappingAction<User, UserWithLinkModel>
    {
        private readonly Func<User?, string?>? _avatarLinkHelper;

        public UserAvatarMapperAction(Services.LinkGeneratorService linkGeneratorService)
        {
            _avatarLinkHelper = linkGeneratorService.AvatarLinkGenerator;
        }

        public void Process(User source, UserWithLinkModel destination, ResolutionContext context)
        {
            destination.Avatar = _avatarLinkHelper == null ? null : _avatarLinkHelper(source);
        }
    }
}
