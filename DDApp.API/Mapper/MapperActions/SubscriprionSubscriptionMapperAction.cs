using AutoMapper;
using DDApp.API.Models.Subscription;
using DDApp.DAL.Entites;

namespace DDApp.API.Mapper.MapperActions
{
    public class SubscriptionSubscriptionMapperAction : IMappingAction<Subscriptions, SubscriptionModel>
    {
        private readonly Func<Avatar?, string?>? _avatarLinkHelper;

        public SubscriptionSubscriptionMapperAction(Services.LinkGeneratorService linkGeneratorService)
        {
            _avatarLinkHelper = linkGeneratorService.AvatarLinkGenerator;
        }

        public void Process(Subscriptions source, SubscriptionModel destination, ResolutionContext context)
        {
            destination.UserAvatar = _avatarLinkHelper == null ? null : _avatarLinkHelper(source.UserSubscription.Avatar);
        }
    }
}
