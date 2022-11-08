using DDApp.API.Models.User;
using DDApp.DAL.Entites;

namespace DDApp.API.Models
{
    public class UserWithLinkModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTimeOffset BirthDate { get; set; }
        public string? Avatar { get; set; }
    }
}
