using AutoMapper;
using DDApp.Common;
using DDApp.API.Models;
using DDApp.API.Mapper.MapperActions;
using DDApp.API.Models.MetaData;
using DDApp.DAL.Entites;

namespace DDApp.API
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CreateUserModel, User>()
                .ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()))
                .ForMember(d => d.PasswordHash, m => m.MapFrom(s => HashHelper.GetHash(s.Password)))
                .ForMember(d => d.BirthDate, m => m.MapFrom(s => s.BirthDate.UtcDateTime));

            CreateMap<User, UserWithLinkModel>().AfterMap<UserAvatarMapperAction>();

            CreateMap<Avatar, AttachModel>();

            CreateMap<PostComments, PostCommentModel>()
                .ForMember(d => d.Likes, m => m.MapFrom(s => s.PostCommentLikes == null ? 0 : s.PostCommentLikes.Count));

            CreateMap<Posts, PostModel>()
                .ForMember(d => d.CommentAmount, m => m.MapFrom(s => s.Comments == null ? 0 : s.Comments.Count))
                .ForMember(d => d.LikesAmount, m => m.MapFrom( s => s.PostLikes == null ? 0 : s.PostLikes.Count))
                .AfterMap<PostAuthorAvatarMapperAction>();

            CreateMap<PostFiles, ExternalPostFileLinkModel>().AfterMap<PostFileMapperAction>();
        }
    }
}
