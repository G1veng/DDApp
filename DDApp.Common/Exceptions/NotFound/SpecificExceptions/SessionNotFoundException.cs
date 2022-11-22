using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDApp.Common.Exceptions.NotFound
{
    public class SessionNotFoundException : NotFoundException
    {
        public SessionNotFoundException()
        {
            Model = "Session";
        }
    }
}
