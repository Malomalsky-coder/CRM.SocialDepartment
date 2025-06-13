using CRM.SocialDepartment.Domain.Entities.Patients.Documents;

namespace CRM.SocialDepartment.Site.ViewModels.Patient
{
    public class DocumentViewModel
    {
        public DocumentType Type { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string InputType { get; set; } = "text";
        public string Placeholder { get; set; } = string.Empty;
        public string Pattern { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
