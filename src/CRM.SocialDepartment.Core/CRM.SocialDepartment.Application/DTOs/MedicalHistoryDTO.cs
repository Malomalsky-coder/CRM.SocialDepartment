using CRM.SocialDepartment.Domain.Entities.Patients;

namespace CRM.SocialDepartment.Application.DTOs
{
    public class MedicalHistoryDTO
    {
        /// <summary>
        /// Номер отделения
        /// </summary>
        public required sbyte NumberDepartment { get; init; }

        /// <summary>
        /// Тип госпитализации
        /// </summary>
        public required HospitalizationTypeDTO HospitalizationType { get; init; }

        /// <summary>
        /// Постановление
        /// </summary>
        public required string Resolution { get; init; }

        /// <summary>
        /// Номер истории болезни
        /// </summary>
        public required string NumberDocument { get; init; }

        /// <summary>
        /// Дата поступления
        /// </summary>
        public required DateTime DateOfReceipt { get; init; }

        /// <summary>
        /// Дата выписки
        /// </summary>
        public DateTime? DateOfDischarge { get; init; }

        /// <summary>
        /// Примечание
        /// </summary>
        public string? Note { get; init; }
    }
}
