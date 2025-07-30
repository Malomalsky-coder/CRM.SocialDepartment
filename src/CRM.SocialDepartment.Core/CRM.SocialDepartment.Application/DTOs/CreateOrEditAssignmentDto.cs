namespace CRM.SocialDepartment.Application.DTOs
{
    public class CreateOrEditAssignmentDto
    {
        public Guid Id { get; init; }
        public string Description { get; init; } = string.Empty;
        public DateTime CreatedDate { get; init; }
        public string ForwardDepartment { get; init; } = string.Empty;
        public DateTime DueDate { get; init; }
        public string Assignee { get; init; } = string.Empty;
        public DateTime AcceptDate { get; init; }
        public DateTime ForwardDate { get; init; }
        public int DepartmentNumber { get; init; }
        public DateTime DepartmentForwardDate { get; init; }
        public string? Note { get; init; }
        public Guid PatientId { get; init; }
    }
}