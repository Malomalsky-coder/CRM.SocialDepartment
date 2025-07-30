namespace CRM.SocialDepartment.Application.DTOs
{
    public class RepresentAssignmentDto
    {
        public Guid Id { get; set; }
        public string Description { get; init; } = string.Empty;
        public DateTime CreateDate { get; set; }
    }
}