using DDApp.DAL.Entites;

namespace DDApp.API.Services
{
    public class LinkGeneratorService
    {
        private Func<Avatar?, string?>? _avatarLinkGenerator;
        private Func<PostFiles?, string?>? _postFileLinkGenerator;
        private Func<Guid?, string?>? _postAuthorAvatarLinkGenerator;

        public Func<Avatar?, string?>? AvatarLinkGenerator { get => _avatarLinkGenerator; set => _avatarLinkGenerator = value; }
        public Func<PostFiles?, string?>? PostFileLinkGenerator { get => _postFileLinkGenerator; set => _postFileLinkGenerator = value; }
        public Func<Guid?, string?>? PostAuthorAvatarLinkGenerator { get => _postAuthorAvatarLinkGenerator; set => _postAuthorAvatarLinkGenerator = value; }

        public LinkGeneratorService()
        {

        }

        public void SetLinkGenerators(Func<Avatar?, string?>? avatarLinkGenerator, Func<PostFiles?, string?>? postFileLinkGenerator,
            Func<Guid?, string?>? postAuthorAvatarLinkGenerator)
        {
            _avatarLinkGenerator = avatarLinkGenerator;
            _postFileLinkGenerator = postFileLinkGenerator;
            _postAuthorAvatarLinkGenerator = postAuthorAvatarLinkGenerator;
        }
    }
}
