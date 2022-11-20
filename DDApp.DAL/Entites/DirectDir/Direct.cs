using DDApp.DAL.Entites.DirectDir;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDApp.DAL.Entites
{
    public class Direct
    {
        public Guid DirectId { get; set; }
        public string DirectTitle { get; set; } = null!;
        public bool IsDirectGroup { get; set; } = false;
        public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;

        public virtual ICollection<DirectMessages>? DirectMessages { get; set; }
        public virtual ICollection<DirectMembers> DirectMembers { get; set; } = null!;
        public virtual DirectImage? DirectImage { get; set; }
    }
}
