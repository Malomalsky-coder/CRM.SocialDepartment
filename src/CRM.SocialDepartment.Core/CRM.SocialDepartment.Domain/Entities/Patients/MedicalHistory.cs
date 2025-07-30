using CRM.SocialDepartment.Domain.Exceptions;
using DDD.Entities;

namespace CRM.SocialDepartment.Domain.Entities.Patients
{
    /// <summary>
    /// Публичный интерфейс для истории болезни
    /// </summary>
    public interface IMedicalHistory
    {
        /// <summary>
        /// Номер отделения
        /// </summary>
        sbyte? NumberDepartment { get; }

        /// <summary>
        /// Тип госпитализации
        /// </summary>
        HospitalizationType HospitalizationType { get; }

        /// <summary>
        /// Дата поступления
        /// </summary>
        DateTime DateOfReceipt { get; }

        /// <summary>
        /// Дата выписки
        /// </summary>
        DateTime? DateOfDischarge { get; }
    }

    /// <summary>
    /// Внутренний интерфейс для истории болезни
    /// </summary>
    internal interface IMedicalHistoryInternal : IMedicalHistory
    {
        void SetNumberDepartment(sbyte numberDepartment);
        void SetDateOfDischarge(DateTime dischargeDate);
    }

    /// <summary>
    /// История болезни
    /// </summary>
    public class MedicalHistory : Entity<Guid>, IMedicalHistoryInternal
    {
        /// <summary>
        /// Номер отделения
        /// </summary>
        public sbyte? NumberDepartment { get; private set; }

        /// <summary>
        /// Тип госпитализации
        /// </summary>
        public HospitalizationType HospitalizationType { get; private set; }

        /// <summary>
        /// Постановление
        /// </summary>
        public string Resolution { get; private set; }

        /// <summary>
        /// Номер истории болезни
        /// </summary>
        public string NumberDocument { get; private set; }

        /// <summary>
        /// Дата поступления
        /// </summary>
        public DateTime DateOfReceipt { get; private set; }

        /// <summary>
        /// Дата выписки
        /// </summary>
        public DateTime? DateOfDischarge { get; private set; }

        /// <summary>
        /// Активная история болезни
        /// </summary>
        public bool IsActive => !DateOfDischarge.HasValue;

        /// <summary>
        /// Примечание
        /// </summary>
        public string? Note { get; private set; }

        #pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        private MedicalHistory() { }
        #pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.

        public MedicalHistory(
            Guid id,
            sbyte numberDepartment,
            HospitalizationType hospitalizationType,
            string resolution,
            string numberDocument,
            DateTime dateOfReceipt,
            string? node)
        {
            Id = id;
            NumberDepartment = numberDepartment;
            HospitalizationType = hospitalizationType;
            Resolution = resolution;
            NumberDocument = numberDocument;
            DateOfReceipt = dateOfReceipt;
            Note = node;
        }

        /// <summary>
        /// Изменить номер отделения, в котором находится пациент
        /// </summary>
        /// <param name="numberDepartment">Новый номер отделения</param>
        void IMedicalHistoryInternal.SetNumberDepartment(sbyte numberDepartment)
        {
            NumberDepartment = numberDepartment;
        }

        /// <summary>
        /// Изменить дату выписки
        /// </summary>
        /// <param name="dischargeDate">Новая дата выписки</param>
        void IMedicalHistoryInternal.SetDateOfDischarge(DateTime dischargeDate)
        {
            if (DateOfReceipt > dischargeDate)
                throw new DomainException("Дата выписки не может быть раньше, чем поступление в больницу");

            if (DateOfDischarge.HasValue)
                throw new DomainException("Дата выписки уже установлена");

            DateOfDischarge = dischargeDate;
        }

        /// <summary>
        /// Изменить примечание
        /// </summary>
        /// <param name="note">Заметка</param>
        public void SetNote(string? note)
        {
            Note = note;
        }
    }
}
