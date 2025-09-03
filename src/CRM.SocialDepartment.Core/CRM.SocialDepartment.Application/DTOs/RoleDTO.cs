namespace CRM.SocialDepartment.Application.DTOs
{
    public class RoleDTO
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string NormalizedName { get; init; } = string.Empty;
        public DateTime CreatedOn { get; init; }
    }
}
