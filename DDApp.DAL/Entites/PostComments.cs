using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDApp.DAL.Entites
{
    public class PostComments
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = null!;
        public DateTimeOffset Created { get; set; }
        public int Likes { get; set; } = 0;

        public virtual Posts Post { get; set; } = null!;
        public virtual User Author { get; set; } = null!;
    }
}
