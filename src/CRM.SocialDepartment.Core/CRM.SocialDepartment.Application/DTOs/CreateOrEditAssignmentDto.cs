using CRM.SocialDepartment.Domain.Entities.Patients;

namespace CRM.SocialDepartment.Application.DTOs;

public class CreateOrEditAssignmentDto
{
    public DateTime AcceptDate { get; set; }
    public int DepartmentNumber { get; set; }
    public string Description { get; set; }
    public DateTime ForwardDate { get; set; }
    public string ForwardDeaprtment { get; set; }
    public DateTime DepartmentForwardDate { get; set; }
    public string Assignee { get; set; }
    public string? Note { get; set; }
    public Patient Patient { get; set; }
}