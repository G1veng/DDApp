using AutoMapper;
using DDApp.API.Models.Subscription;
using DDApp.DAL.Entites;

namespace DDApp.API.Mapper.MapperActions
{
    public class SubscriptionAvatarMapperAction : IMappingAction<User, SubscriberModel>
    {
        private readonly Func<Avatar?, string?>? _avatarLinkHelper;

        public SubscriptionAvatarMapperAction(Services.LinkGeneratorService linkGeneratorService)
        {
            _avatarLinkHelper = linkGeneratorService.AvatarLinkGenerator;
        }

        public void Process(User source, SubscriberModel destination, ResolutionContext context)
        {
            destination.UserAvatar = _avatarLinkHelper == null ? null : _avatarLinkHelper(source.Avatar);
        }
    }
}
