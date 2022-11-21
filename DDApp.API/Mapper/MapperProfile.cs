using AutoMapper;
using DDApp.Common;
using DDApp.API.Models;
using DDApp.API.Mapper.MapperActions;
using DDApp.API.Models.MetaData;
using DDApp.DAL.Entites;
using DDApp.API.Models.Direct;
using DDApp.DAL.Entites.DirectDir;

namespace DDApp.API
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CreateUserModel, User>()
                .ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()))
                .ForMember(d => d.PasswordHash, m => m.MapFrom(s => HashHelper.GetHash(s.Password)))
                .ForMember(d => d.BirthDate, m => m.MapFrom(s => s.BirthDate.UtcDateTime))
                .ForMember(d => d.Created, m => m.MapFrom(s => DateTimeOffset.UtcNow));

            CreateMap<User, UserWithLinkModel>().AfterMap<UserAvatarMapperAction>();

            CreateMap<Avatar, AttachModel>();

            CreateMap<PostComments, PostCommentModel>()
                .ForMember(d => d.Likes, m => m.MapFrom(s => s.PostCommentLikes == null ? 0 : s.PostCommentLikes.Count));

            CreateMap<Posts, PostModel>()
                .ForMember(d => d.CommentAmount, m => m.MapFrom(s => s.Comments == null ? 0 : s.Comments.Count))
                .ForMember(d => d.LikesAmount, m => m.MapFrom( s => s.PostLikes == null ? 0 : s.PostLikes.Count))
                .AfterMap<PostAuthorAvatarMapperAction>();

            CreateMap<PostFiles, ExternalPostFileLinkModel>().AfterMap<PostFileMapperAction>();

            CreateMap<Subscriptions, Models.Subscription.SubscriptionModel>()
                .ForMember(d => d.LastEntered, m => m.MapFrom(s => (s.UserSubscription.Session == null || s.UserSubscription.Session.Count == 0) ? s.UserSubscription.Created : s.UserSubscription.Session.Last().Created))
                .ForMember(d => d.UserId, m => m.MapFrom(s => s.UserSubscription.Id))
                .ForMember(d => d.UserName, m => m.MapFrom(s => s.UserSubscription.Name))
                .AfterMap<SubscriptionSubscriptionMapperAction>();

            CreateMap<Subscriptions, Models.Subscription.SubscriberModel>()
                .ForMember(d => d.LastEntered, m => m.MapFrom(s => (s.UserSubscriber.Session == null || s.UserSubscriber.Session.Count == 0) ? s.UserSubscriber.Created : s.UserSubscriber.Session.Last().Created))
                .ForMember(d => d.UserId, m => m.MapFrom(s => s.UserSubscriber.Id))
                .ForMember(d => d.UserName, m => m.MapFrom(s => s.UserSubscriber.Name))
                .AfterMap<SubscriptionSubscriberMapperAction>();



            /*CreateMap<Direct, DDApp.API.Models.Direct.DirectRequestWithSenderModel>()
                .ForMember(x => x.DirectImage, m => m.MapFrom(s => s.DirectImage));

            CreateMap<DirectRequestWithSenderModel, DirectModel>()
                .ForMember(x => x.RecipientId, m => m.MapFrom(s => s.Recipient.Id))
                .ForMember(x => x.RecipientUserName, m => m.MapFrom(s => s.Recipient.Name))
                .AfterMap<DirectImageMapperAction>();*/

            /*CreateMap<DirectMessages, DirectMessageModel>()
                .ForMember(x => x.SenderName, m => m.MapFrom(s => s.User.Name));

            CreateMap<DirectFiles, ExternalDirectFileLinkModel>()
                .AfterMap<DirectMessageFileMapperAction>();

            CreateMap<Direct, DirectGroupRequestModel>();*/

            CreateMap<Direct, DirectModel>()
                .BeforeMap<DirectModelImageMapperAction>();

            CreateMap<DirectImages, DirectImageModel>()
                .AfterMap<DirectGroupImageMapperAction>();

            CreateMap<DirectMembers, DirectMemberModel>()
                .ForMember(d => d.DirectMember, m => m.MapFrom(s => s.UserId));
        }
    }
}
