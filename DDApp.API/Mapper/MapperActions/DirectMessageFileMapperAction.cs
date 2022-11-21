using AutoMapper;
using DDApp.API.Models.Direct;
using DDApp.API.Models.MetaData;
using DDApp.DAL.Entites;

namespace DDApp.API.Mapper.MapperActions
{
    public class DirectMessageFileMapperAction : IMappingAction<DirectFiles, ExternalDirectFileLinkModel>
    {
        private readonly Func<DirectFiles?, string?>? _directImageLinkHelper;

        public DirectMessageFileMapperAction(Services.LinkGeneratorService linkGeneratorService)
        {
            _directImageLinkHelper = linkGeneratorService.DirectFilesLinkGenerator;
        }
        public void Process(DirectFiles source, ExternalDirectFileLinkModel destination, ResolutionContext context)
        {
            destination.Link = _directImageLinkHelper == null ? null : _directImageLinkHelper(source);
        }
    }
}
