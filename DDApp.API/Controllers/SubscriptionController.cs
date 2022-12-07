using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DDApp.API.Services;
using DDApp.Common.Exceptions;
using DDApp.Common.Consts;
using DDApp.Common.Extensions;
using DDApp.API.Models.Subscription;
using Microsoft.AspNetCore.Authorization;
using DDApp.Common.Exceptions.Authorization;

namespace DDApp.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Api")]
    [Authorize]
    public class SubscriptionController : ControllerBase
    {
        SubscriptionService _subscriptionService;

        public SubscriptionController(SubscriptionService subscriptionService, LinkGeneratorService linkGeneratorService)
        {
            _subscriptionService = subscriptionService;
            linkGeneratorService.AvatarLinkGenerator =
                x => Url.ControllerAction<AttachController>(nameof(AttachController.GetUserAvatarByAttachId), new { attachId = x?.Id });
        }

        [HttpPost]
        public async Task ChangeSubscriptionStateOnUser(Guid userId)
            => await _subscriptionService.ChangeSubscriptionStateOnUserById(GetCurrentUserGuid(), userId);

        [HttpGet]
        public async Task<List<SubscriberModel>?> GetSubscribers(int take = 10, int skip = 0)
            => await _subscriptionService.GetSubscribers(GetCurrentUserGuid(), skip, take);

        [HttpGet]
        public async Task<List<SubscriptionModel>?> GetSubscriptions(int take = 10, int skip = 0)
            => await _subscriptionService.GetSubscriptions(GetCurrentUserGuid(), skip, take);

        [HttpGet]
        public async Task<int> GetUserSubscriptionsAmount()
            => await _subscriptionService.GetUserSubscriptionsAmount(GetCurrentUserGuid());

        [HttpGet]
        public async Task<int> GetUserSubscribersAmount()
            => await _subscriptionService.GetUserSubscribersAmount(GetCurrentUserGuid());

        private Guid GetCurrentUserGuid()
        {
            var userIdString = User.Claims.FirstOrDefault(x => x.Type == Claims.Id)?.Value;

            if (!Guid.TryParse(userIdString, out var userId))
            {
                throw new UserAuthorizationException();
            }
            else
            {
                return userId;
            }
        }
    }
}
