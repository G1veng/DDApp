using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDApp.DAL.Entites.DirectDir
{
    public class DirectImages : Attach
    {
        public Guid DirectId { get; set; }

        public virtual Direct DirectImg { get; set; } = null!;
    }
}
