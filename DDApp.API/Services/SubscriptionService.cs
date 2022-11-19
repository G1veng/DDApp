using AutoMapper;
using DDApp.API.Models.Subscription;
using DDApp.Common.Exceptions;
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
        {
            var subscribersDb = await _context.Subscriptions
                .AsNoTracking()
                .Skip(skip)
                .Take(take)
                .Where(x => x.SubscriptionId == userId)
                .ToListAsync();

            if(subscribersDb == null || subscribersDb.Count == 0)
            {
                return new List<SubscriberModel>();
            }

            var subscribers = new List<SubscriberModel>();

            subscribersDb.ForEach(x =>
            {
                subscribers.Add(
                    _mapper.Map<SubscriberModel>(_context.Users
                    .Include(y => y.Avatar)
                    .Include(y => y.Session)
                    .AsNoTracking()
                    .FirstOrDefault(y => y.Id == x.SubscriberId))
                );
            });

            return subscribers;
        }

        public async Task ChangeSubscriptionStateOnUserById(Guid subscriberId, Guid subscriptionId)
        {
            if(subscriberId == subscriptionId)
            {
                throw new UserException("Can't subscribe on yourself");
            }

            if(await CheckUserExistById(subscriberId) == false || await CheckUserExistById(subscriptionId) == false)
            {
                throw new UserException("User not found");
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

        public async Task<List<SubscriberModel>> GetSubscriptions(Guid userId, int skip, int take)
        {
            var subscriptionsDb = await _context.Subscriptions
                .AsNoTracking()
                .Skip(skip)
                .Take(take)
                .Where(x => x.SubscriberId == userId)
                .ToListAsync();

            if (subscriptionsDb == null || subscriptionsDb.Count == 0)
            {
                return new List<SubscriberModel>();
            }

            var subscriptions = new List<SubscriberModel>();

            subscriptionsDb.ForEach(x =>
            {
                subscriptions.Add(_mapper.Map<SubscriberModel>(
                    _context.Users
                    .Include(y => y.Avatar)
                    .Include(y => y.Session)
                    .AsNoTracking()
                    .FirstOrDefault(y => y.Id == x.SubscriptionId))
                );
            });

            return subscriptions;
        }


        private async Task<bool> CheckUserExistById(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null || user.IsActive == false || user == default)
            {
                return false;
            }

            return true;
        }

        private async Task<User> GetUserById(Guid id)
        {
            var user = await _context.Users.Include(x => x.Avatar).FirstOrDefaultAsync(x => x.Id == id);
            if (user == null || user.IsActive == false)
            {
                throw new UserException("User not found");
            }

            return user;
        }
    }
}
