using DDApp.DAL.Entites;

namespace DDApp.API.Models.User
{
    public class UserRequestModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTimeOffset BirthDate { get; set; }
        public Avatar? Avatar { get; set; }
        public Func<Avatar?, string?>? LinkGenerator { get; set; }
    }
}

