using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDApp.Common.Exceptions.Authorization
{
    public class UserAuthorizationException : AuthorizationException
    {
        public UserAuthorizationException()
        {
            Model = "User";
        }
    }
}
