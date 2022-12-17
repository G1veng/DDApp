using DDApp.API.Models.MetaData;
using DDApp.DAL.Entites;
using DDApp.DAL.Entites.DirectDir;

namespace DDApp.API.Services
{
    public class LinkGeneratorService
    {
        private Func<User?, string?>? _avatarLinkGenerator;
        private Func<PostFiles?, string?>? _postFileLinkGenerator;
        private Func<Guid?, string?>? _postAuthorAvatarLinkGenerator;
        private Func<Attach?, string?>? _directImageLinkGenerator;
        private Func<DirectFiles?, string?>? _directFilesLinkGenerator;
        public Func<DirectImages?, string?>? _directGroupImageLinkGenerator;

        public Func<User?, string?>? AvatarLinkGenerator { get => _avatarLinkGenerator; set => _avatarLinkGenerator = value; }
        public Func<PostFiles?, string?>? PostFileLinkGenerator { get => _postFileLinkGenerator; set => _postFileLinkGenerator = value; }
        public Func<Guid?, string?>? PostAuthorAvatarLinkGenerator { get => _postAuthorAvatarLinkGenerator; set => _postAuthorAvatarLinkGenerator = value; }
        public Func<Attach?, string?>? DirectImageLinkGenerator { get => _directImageLinkGenerator; set => _directImageLinkGenerator = value; }
        public Func<DirectFiles?, string?>? DirectFilesLinkGenerator { get => _directFilesLinkGenerator; set => _directFilesLinkGenerator = value; }
        public Func<DirectImages?, string?>? DirectGroupImageLinkGenerator { get => _directGroupImageLinkGenerator; set => _directGroupImageLinkGenerator = value; }

        public LinkGeneratorService()
        {

        }
    }
}
