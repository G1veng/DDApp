using AutoMapper;
using DDApp.Common;
using DDApp.API.Models.User;
using DDApp.API.Models;

namespace DDApp.API
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Models.CreateUserModel, DDApp.DAL.Entites.User>()
                .ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()))
                .ForMember(d => d.PasswordHash, m => m.MapFrom(s => HashHelper.GetHash(s.Password)))
                .ForMember(d => d.BirthDate, m => m.MapFrom(s => s.BirthDate.UtcDateTime))
                ;

            CreateMap<DDApp.DAL.Entites.User, UserRequestModel>();

            CreateMap<UserRequestModel, UserWithLinkModel>()
                .ForMember(d=> d.Avatar, m => m.MapFrom(s=> s.LinkGenerator == null ? null : s.LinkGenerator(s.Avatar)));

            CreateMap<DAL.Entites.Avatar, Models.AttachModel>();

            CreateMap<Models.Post.RequestPostModel, Models.PostModel>();

            CreateMap<DAL.Entites.PostComments, Models.PostCommentModel>();

            CreateMap<DAL.Entites.Posts, Models.Post.RequestPostModel>()
                .ForMember(d => d.CommentAmount, m => m.MapFrom(s => s.Comments == null ? 0 : s.Comments.Count));
        }
    }
}
