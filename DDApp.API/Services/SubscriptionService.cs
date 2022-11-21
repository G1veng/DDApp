using AutoMapper;
using DDApp.API.Models.Subscription;
using DDApp.Common.Exceptions;
using DDApp.Common.Exceptions.Forbidden;
using DDApp.DAL;
using DDApp.DAL.Entites;
using Microsoft.EntityFrameworkCore;

namespace DDApp.API.Services
{
    public class SubscriptionService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public SubscriptionService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<SubscriberModel>> GetSubscribers(Guid userId, int skip, int take)
            => await _context.Subscriptions
                .AsNoTracking()
                .Skip(skip)
                .Take(take)
                .Where(x => x.SubscriptionId == userId)
                .Include(x => x.UserSubscriber).ThenInclude(x => x.Avatar)
                .Include(x => x.UserSubscriber).ThenInclude(x => x.Session)
                .Select(x => _mapper.Map<SubscriberModel>(x))
                .ToListAsync();

        public async Task ChangeSubscriptionStateOnUserById(Guid subscriberId, Guid subscriptionId)
        {
            if(subscriberId == subscriptionId)
            {
                throw new UserSubscriptionForbiddentException();
            }

            if(await CheckUserExistById(subscriberId) == false || await CheckUserExistById(subscriptionId) == false)
            {
                throw new UserNotFoundException();
            }

            var subscription = new Subscriptions
            {
                SubscriberId = subscriberId,
                SubscriptionId = subscriptionId,
            };

            if (_context.Subscriptions.Contains(subscription))
            {
                _context.Subscriptions.Remove(subscription);
            }
            else
            {
                await _context.Subscriptions.AddAsync(subscription);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<SubscriptionModel>> GetSubscriptions(Guid userId, int skip, int take)
            => await _context.Subscriptions
                .AsNoTracking()
                .Skip(skip)
                .Take(take)
                .Where(x => x.SubscriberId == userId)
                .Include(x => x.UserSubscription).ThenInclude(x => x.Avatar)
                .Include(x => x.UserSubscription).ThenInclude(x => x.Session)
                .Select(x => _mapper.Map<SubscriptionModel>(x))
                .ToListAsync();


        private async Task<bool> CheckUserExistById(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null || user.IsActive == false || user == default)
            {
                return false;
            }

            return true;
        }
    }
}
