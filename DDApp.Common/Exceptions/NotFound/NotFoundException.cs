using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDApp.Common.Exceptions
{
    public class NotFoundException : Exception 
    {
        public string? Model { get; set; }
        public override string Message => $"{Model} is not found";
    } 
}
