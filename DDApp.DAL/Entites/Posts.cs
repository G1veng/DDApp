using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDApp.DAL.Entites
{
    public class Posts
    {
        public Guid Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public string Text { get; set; } = null!;

        public virtual ICollection<PostComments>? Comments { get; set; }
        public virtual ICollection<PostFiles>? PostPhotos { get; set; }
        public virtual User Author { get; set; } = null!;
    }
}
