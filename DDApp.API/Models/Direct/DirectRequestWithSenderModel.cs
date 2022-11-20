using DDApp.DAL.Entites;
using DDApp.DAL.Entites.DirectDir;

namespace DDApp.API.Models.Direct
{
    public class DirectRequestWithSenderModel
    {
        public Attach? DirectImage { get; set; }
        public User Recipient { get; set; } = null!;
        public Guid DirectId { get; set; }
        public string DirectTitle { get; set; } = null!;
        public bool IsDirectGroup { get; set; } = false;
        public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;
    }
}
