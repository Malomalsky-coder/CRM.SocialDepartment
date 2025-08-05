namespace CRM.SocialDepartment.Application.DTOs
{
    public class UserDTO
    {
        //public string Id { get; init; } = string.Empty;
        public string UserName { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public DateTime CreatedOn { get; init; }
    }
}
