using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDApp.Common.Exceptions.UnsopportedMediaType
{
    public class UnsopportedMediaTypeException : Exception
    {
        public string? Model { get; set; }
        public override string Message => $"File is not {Model}";
    }
}
