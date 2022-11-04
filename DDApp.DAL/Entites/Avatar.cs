using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDApp.DAL.Entites
{
    public class Avatar : Attach
    {
        public virtual User UserId { get; set; } = null!;
    }
}
