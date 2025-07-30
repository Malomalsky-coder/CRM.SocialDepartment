using DDD.Values;

namespace CRM.SocialDepartment.Domain.Entities.Patients.Documents
{
    public class MedicalPolicyDocument : DocumentType
    {
        public string? Number { get; }

        public MedicalPolicyDocument(string? number) : base(1, "Медицинский полис")
        {
            Number = number;
        }

        protected MedicalPolicyDocument() : base(1, "Медицинский полис")
        {
            Number = null;
        }

        public bool IsValid()
        {
            // Простая валидация формата полиса: 16 цифр
            return !string.IsNullOrEmpty(Number) && System.Text.RegularExpressions.Regex.IsMatch(Number, @"^\d{16}$");
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Number ?? string.Empty;
        }

        public override string ToString() => $"Медицинский полис: {Number ?? "не указан"}";
    }
}
