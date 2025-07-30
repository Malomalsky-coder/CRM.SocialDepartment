using DDD.Values;

namespace CRM.SocialDepartment.Domain.Entities.Patients.Documents
{
    public class SnilsDocument : DocumentType
    {
        public string? Number { get; }

        public SnilsDocument(string? number) : base(2, "СНИЛС")
        {
            Number = number;
        }

        protected SnilsDocument() : base(2, "СНИЛС")
        {
            Number = null;
        }

        public bool IsValid()
        {
            // Простая валидация формата СНИЛС: 3 цифры-3 цифры-3 цифры 2 цифры
            return !string.IsNullOrEmpty(Number) && System.Text.RegularExpressions.Regex.IsMatch(Number, @"^\d{3}-\d{3}-\d{3}\s\d{2}$");
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Number ?? string.Empty;
        }

        public override string ToString() => $"СНИЛС: {Number ?? "не указан"}";
    }
}