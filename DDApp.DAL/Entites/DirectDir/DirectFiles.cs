using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDApp.DAL.Entites
{
    public class DirectFiles : Attach
    {
        public Guid DirectMessagesId { get; set; }

        public virtual DirectMessages DirectMessage { get; set; } = null!;
    }
}
