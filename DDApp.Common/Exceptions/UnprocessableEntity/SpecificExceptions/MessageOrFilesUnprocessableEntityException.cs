using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDApp.Common.Exceptions.UnprocessableEntity
{
    public class MessageOrFilesUnprocessableEntityException : UnprocessableEntityException
    {
        public MessageOrFilesUnprocessableEntityException()
        {
            Model = "Files or message field must be entered";
        }
    }
}
