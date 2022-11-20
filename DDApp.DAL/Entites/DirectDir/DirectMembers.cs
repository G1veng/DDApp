using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDApp.DAL.Entites
{
    public class DirectMembers
    {
        public Guid DirectId { get; set; }
        public Guid UserId { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual Direct Direct { get; set; } = null!;
    }
}
