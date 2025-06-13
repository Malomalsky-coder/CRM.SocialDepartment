namespace CRM.SocialDepartment.Application.DTOs
{
    public class CreateCapableDTO
    {
        public required string CourtDecision { get; init; }
        public required DateTime TrialDate { get; init; }
        public required string Guardian { get; init; }
        public required string GuardianOrderAppointment { get; init; }
    }
}
