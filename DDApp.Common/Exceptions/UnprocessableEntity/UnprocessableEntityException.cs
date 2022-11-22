using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDApp.Common.Exceptions.UnprocessableEntity
{
    public class UnprocessableEntityException : Exception
    {
        public string? Model { get; set; }

        public override string Message => $"Semantic error, required {Model}";
    }
}
