using AutoMapper;
using DDApp.API.Models.Subscription;
using DDApp.DAL.Entites;

namespace DDApp.API.Mapper.MapperActions
{
    

    public class SubscriptionSubscriberMapperAction : IMappingAction<Subscriptions, SubscriberModel>
    {
        private readonly Func<User?, string?>? _avatarLinkHelper;

        public SubscriptionSubscriberMapperAction(Services.LinkGeneratorService linkGeneratorService)
        {
            _avatarLinkHelper = linkGeneratorService.AvatarLinkGenerator;
        }

        public void Process(Subscriptions source, SubscriberModel destination, ResolutionContext context)
        {
            destination.UserAvatar = _avatarLinkHelper == null ? null : _avatarLinkHelper(source.UserSubscriber);
        }
    }
}
