using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDApp.Common.Exceptions
{
    public class WrongTypeException : Exception
    {
        public WrongTypeException(string message) : base(message) { }
    }
}
