using AutoMapper;
using DDApp.API.Models;
using DDApp.API.Models.MetaData;
using DDApp.API.Models.Post;
using DDApp.DAL.Entites;

namespace DDApp.API.Mapper.MapperActions
{
    public class PostFileMapperAction : IMappingAction<PostFiles, ExternalPostFileLinkModel>
    {
        private readonly Func<PostFiles?, string?>? _postFileLinkHelper;

        public PostFileMapperAction(Services.LinkGeneratorService linkGeneratorService)
        {
            _postFileLinkHelper = linkGeneratorService.PostFileLinkGenerator;
        }

        public void Process(PostFiles source, ExternalPostFileLinkModel destination, ResolutionContext context)
        {
            destination.Link = _postFileLinkHelper == null ? null : _postFileLinkHelper(source);
        }
    }

    public class PostFileMapperActionSecond : IMappingAction<PostFiles, ExternalPostFileLinkModel>
    {
        private readonly Func<PostFiles?, string?>? _postFileLinkHelper;

        public PostFileMapperActionSecond(Services.LinkGeneratorService linkGeneratorService)
        {
            _postFileLinkHelper = linkGeneratorService.PostFileLinkGenerator;
        }

        public void Process(PostFiles source, ExternalPostFileLinkModel destination, ResolutionContext context)
        {
            destination.Link = _postFileLinkHelper == null ? null : _postFileLinkHelper(source);
        }
    }
}
