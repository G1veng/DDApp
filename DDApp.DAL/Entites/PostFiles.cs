using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDApp.DAL.Entites
{
    public class PostFiles : Attach
    {
        public Guid PostId { get; set; }

        public virtual Posts Post { get; set; } = null!;
    }
}
