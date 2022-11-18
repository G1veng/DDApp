using AutoMapper;
using DDApp.API.Models;
using DDApp.DAL.Entites;

namespace DDApp.API.Mapper.MapperActions
{
    public class PostAuthorAvatarMapperAction : IMappingAction<Posts, PostModel>
    {
        private readonly Func<Guid?, string?>? _postFileLinkHelper;

        public PostAuthorAvatarMapperAction(Services.LinkGeneratorService linkGeneratorService)
        {
            _postFileLinkHelper = linkGeneratorService.PostAuthorAvatarLinkGenerator;
        }

        public void Process(Posts source, PostModel destination, ResolutionContext context)
        {
            destination.AuthorAvatar = _postFileLinkHelper == null ? null : _postFileLinkHelper(source.Author.Id);
        }
    }
}
