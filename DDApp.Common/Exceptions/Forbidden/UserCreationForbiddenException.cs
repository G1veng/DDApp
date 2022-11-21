using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDApp.Common.Exceptions.Forbidden
{
    public class UserCreationForbiddenException : ForbiddenException
    {
        public UserCreationForbiddenException()
        {
            Model = "Repeated user creation";
        }
    }
}
