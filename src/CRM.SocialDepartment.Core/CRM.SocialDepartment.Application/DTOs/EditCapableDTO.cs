namespace CRM.SocialDepartment.Application.DTOs
{
    public class EditCapableDTO
    {
        public string CourtDecision { get; init; } = string.Empty;
        public DateTime? TrialDate { get; init; } = DateTime.MinValue;
        public string Guardian { get; init; } = string.Empty;
        public string GuardianOrderAppointment { get; init; } = string.Empty;
    }
}
