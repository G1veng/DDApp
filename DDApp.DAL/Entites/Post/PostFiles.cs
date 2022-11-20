namespace DDApp.DAL.Entites
{
    public class PostFiles : Attach
    {
        public Guid PostId { get; set; }
        public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;

        public virtual Posts Post { get; set; } = null!;
    }
}
