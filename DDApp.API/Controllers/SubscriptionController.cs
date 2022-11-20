using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DDApp.API.Services;
using DDApp.Common.Exceptions;
using DDApp.Common.Consts;
using DDApp.Common.Extensions;
using DDApp.API.Models.Subscription;
using Microsoft.AspNetCore.Authorization;

namespace DDApp.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Api")]
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
        [Authorize]
        public async Task ChangeSubscriptionStateOnUser(Guid subscriptionId)
            => await _subscriptionService.ChangeSubscriptionStateOnUserById(GetCurrentUserGuid(), subscriptionId);

        [HttpGet]
        [Authorize]
        public async Task<List<SubscriberModel>?> GetSubscribers(int skip = 0, int take = 10)
            => await _subscriptionService.GetSubscribers(GetCurrentUserGuid(), skip, take);

        [HttpGet]
        [Authorize]
        public async Task<List<SubscriptionModel>?> GetSubscriptions(int skip = 0, int take = 10)
            => await _subscriptionService.GetSubscriptions(GetCurrentUserGuid(), skip, take);


        private Guid GetCurrentUserGuid()
        {
            var userIdString = User.Claims.FirstOrDefault(x => x.Type == Claims.Id)?.Value;

            if (!Guid.TryParse(userIdString, out var userId))
            {
                throw new AuthorizationException("You are not authorized");
            }
            else
            {
                return userId;
            }
        }
    }
}
