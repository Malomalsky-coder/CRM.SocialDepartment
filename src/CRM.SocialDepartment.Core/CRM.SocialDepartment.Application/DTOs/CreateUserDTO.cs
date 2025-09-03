
namespace CRM.SocialDepartment.Application.DTOs
{
    public class CreateUserDTO
    {
        public string UserName { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string ConfirmPassword { get; init; } = string.Empty;
        public string Role { get; init; } = string.Empty;
        public string Position { get; init; } = string.Empty;
        public string DepartmentNumber { get; init; } = string.Empty;
    }
}
