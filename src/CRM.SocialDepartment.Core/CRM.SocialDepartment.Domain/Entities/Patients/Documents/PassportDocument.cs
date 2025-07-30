using DDD.Values;

namespace CRM.SocialDepartment.Domain.Entities.Patients.Documents
{
    public class PassportDocument : DocumentType
    {
        public string? Number { get; }

        public PassportDocument(string? number) : base(0, "Паспорт")
        {
            Number = number;
        }

        protected PassportDocument() : base(0, "Паспорт")
        {
            Number = null;
        }

        public bool IsValid()
        {
            // Простая валидация формата паспорта: 4 цифры, пробел, 6 цифр
            return !string.IsNullOrEmpty(Number) && System.Text.RegularExpressions.Regex.IsMatch(Number, @"^\d{4}\s\d{6}$");
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Number ?? string.Empty;
        }

        public override string ToString() => $"Паспорт: {Number ?? "не указан"}";
    }
}
