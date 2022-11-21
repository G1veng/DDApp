using AutoMapper;
using DDApp.API.Models.Direct;
using DDApp.API.Services;
using DDApp.DAL.Entites.DirectDir;

namespace DDApp.API.Mapper.MapperActions
{
    public class DirectGroupImageMapperAction : IMappingAction<DirectImages, DirectImageModel>
    {
        private readonly Func<DirectImages?, string?>? _directImageLinkHelper;

        public DirectGroupImageMapperAction(LinkGeneratorService linkGeneratorService) 
        {
            _directImageLinkHelper = linkGeneratorService.DirectGroupImageLinkGenerator;
        }

        public void Process(DirectImages source, DirectImageModel destination, ResolutionContext context)
        {
            destination.Link = _directImageLinkHelper == null ? null : _directImageLinkHelper(source);
        }
    }
}
