using AutoMapper;
using DDApp.API.Models.Direct;
using DDApp.DAL.Entites;
using DDApp.DAL.Entites.DirectDir;

namespace DDApp.API.Mapper.MapperActions
{
    public class DirectImageMapperAction : IMappingAction<DirectRequestWithSenderModel, DirectModel>
    {
        private readonly Func<Attach?, string?>? _directImageLinkHelper;

        public DirectImageMapperAction(Services.LinkGeneratorService linkGeneratorService)
        {
            _directImageLinkHelper = linkGeneratorService.DirectImageLinkGenerator;
        }

        public void Process(DirectRequestWithSenderModel source, DirectModel destination, ResolutionContext context)
        {
            destination.DirectImage = _directImageLinkHelper == null ? null : _directImageLinkHelper(source.DirectImage);
        }
    }
}