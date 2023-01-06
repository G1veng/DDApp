using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDApp.Common.Exceptions.Authorization.SpecificExceptions
{
    public class AccessAuthorizationException : AuthorizationException
    {
        public AccessAuthorizationException()
        {
            Model = "Access";
        }
    }
}
