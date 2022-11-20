using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDApp.DAL.Entites
{
    public class DirectMessages
    {
        public Guid DirectMessageId { get; set; }
        public Guid DirectId { get; set; }
        public string? DirectMessage { get; set; }
        public DateTimeOffset Sended { get; set; } = DateTimeOffset.UtcNow;
        public Guid SenderId { get; set; }

        public virtual Direct Direct { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual ICollection<DirectFiles>? DirectFiles { get; set; }
    }
}
