using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDApp.Common.Exceptions.UnsopportedMediaType
{
    public class NotImageFileException : UnsopportedMediaTypeException
    {
        public NotImageFileException()
        {
            Model = "File";
        }
    }
}
