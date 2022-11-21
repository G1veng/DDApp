using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDApp.Common.Exceptions.NotFound
{
    public class AvatarNotFoundException : NotFoundException
    {
        public AvatarNotFoundException()
        {
            Model = "Avatar";
        }
    }
}
