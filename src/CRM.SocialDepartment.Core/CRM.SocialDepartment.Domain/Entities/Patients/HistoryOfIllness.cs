using DDD.Entities;

namespace CRM.SocialDepartment.Domain.Entities.Patients
{
    /// <summary>
    /// История болезни
    /// </summary>
    public class HistoryOfIllness : AggregateRoot<Guid>
    {
        /// <summary>
        /// Номер отделения
        /// </summary>
        public sbyte NumberDepartment { get; private set; }

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
        public bool IsActive => !DateOfDischarge.HasValue; //TODO: Нужно быть внимательным, не должно быть несколько активный историй болезней.

        /// <summary>
        /// Примечание
        /// </summary>
        public string? Note { get; private set; }

        /// <summary>
        /// Уникальный индетификатор пациента, служит связью между колекциями
        /// </summary>
        public Guid PatientId { get; private set; }

        #pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        private HistoryOfIllness() { }
        #pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.

        public HistoryOfIllness(
            HospitalizationType hospitalizationType,
            string resolution,
            string numberDocument,
            DateTime dateOfReceipt,
            DateTime? dateOfDischarge,
            string? note,
            Guid patientId)
        {
            HospitalizationType = hospitalizationType;
            Resolution = resolution;
            NumberDocument = numberDocument;
            DateOfReceipt = dateOfReceipt;
            DateOfDischarge = dateOfDischarge;
            Note = note;
            PatientId = patientId;
        }

        /// <summary>
        /// Изменить номер отделения, в котором находится пациент
        /// </summary>
        /// <param name="number"></param>
        public void SetNumberDepartment(sbyte number)
        {
            NumberDepartment = number;
        }

        /// <summary>
        /// Изменить дату выписки
        /// </summary>
        /// <param name="dateOfDischarge"></param>
        public void SetDateOfDischarge(DateTime dateOfDischarge)
        {
            if (DateOfDischarge is null)
                DateOfDischarge = dateOfDischarge;
        }

        /// <summary>
        /// Изменить примечание
        /// </summary>
        /// <param name="note"></param>
        public void SetNote(string note)
        {
            Note = note;
        }
    }
}
