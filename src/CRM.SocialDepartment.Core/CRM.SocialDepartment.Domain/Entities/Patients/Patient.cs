using CRM.SocialDepartment.Domain.Entities.Patients.Documents;
using CRM.SocialDepartment.Domain.Exceptions;
using DDD.Entities;
using System.Reflection.Metadata;

namespace CRM.SocialDepartment.Domain.Entities.Patients
{
    public class Patient : AggregateRoot<Guid>, IArchive, ISoftDelete
    {
        /// <summary>
        /// Полное имя пациента
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        /// Дата рождения
        /// </summary>
        public DateTime Birthday { get; private set; }

        /// <summary>
        /// Несовершеннолетний
        /// </summary>
        public bool IsChildren => (DateTime.Now - Birthday).TotalDays < 365 * 18; // 18 лет в днях

        /// <summary>
        /// Информация о гражданстве
        /// </summary>
        public CitizenshipInfo CitizenshipInfo { get; private set; }

        /// <summary>
        /// Список документов
        /// </summary>
        public Dictionary<DocumentType, Document> Documents { get; private set; }

        /// <summary>
        /// Пациент является дееспособным?
        /// </summary>
        public bool IsCapable => Capable is not null;

        /// <summary>
        /// Дееспособный
        /// </summary>
        public Capable? Capable { get; private set; }

        /// <summary>
        /// Получает ли пациент пенсию
        /// </summary>
        public bool ReceivesPension => Pension is not null;

        /// <summary>
        /// Пенсия
        /// </summary>
        public Pension? Pension { get; private set; }

        /// <summary>
        /// Примечание
        /// </summary>
        public string? Note { get; private set; }

        /// <summary>
        /// Помечен как удаленный
        /// </summary>
        public bool SoftDeleted { get; set; }

        /// <summary>
        /// Помечен как в архиве (пациент выписан)
        /// </summary>
        public bool IsArchive { get; set; }

        private readonly List<MedicalHistory> _medicalHistories;

        /// <summary>
        /// Все истории болезни пациента
        /// </summary>
        public IReadOnlyList<MedicalHistory> MedicalHistories => _medicalHistories.AsReadOnly();

        /// <summary>
        /// Активная история болезни
        /// </summary>
        public MedicalHistory? ActiveHistory => _medicalHistories.FirstOrDefault(h => !h.DateOfDischarge.HasValue);

        #pragma warning disable CS8618
        private Patient() 
        {
            _medicalHistories = new List<MedicalHistory>();
        }
        #pragma warning restore CS8618

        public Patient(
            Guid id,
            string fullname,
            DateTime birthday,
            MedicalHistory medicalHistory,
            CitizenshipInfo citizenshipInfo,
            Capable? capable,
            Pension? pension,
            string? note
            )
        {
            Id = id;
            FullName = fullname;
            Birthday = birthday;
            _medicalHistories = new List<MedicalHistory> { medicalHistory };
            CitizenshipInfo = citizenshipInfo;
            Documents = new Dictionary<DocumentType, Document>();
            Capable = capable;
            Pension = pension;
            Note = note;
        }

        /// <summary>
        /// Изменить фио
        /// </summary>
        /// <param name="fullName"></param>
        /// <exception cref="DomainException">Если имя равно null или пустой строке</exception>
        public void ChangeFullName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrWhiteSpace(fullName))
                throw new DomainException("Имя не может быть null или пустой строкой");

            FullName = fullName;
        }

        /// <summary>
        /// Изменить информацию о гражданстве
        /// </summary>
        /// <param name="citizenship">Гражданство</param>
        /// <param name="country">Страна</param>
        /// <param name="registration">Регистрация</param>
        /// <param name="earlyRegistration">Ранняя регистрация</param>
        /// <param name="placeOfBirth">Место рождения</param>
        public void ChangeCitizenshipInfo(CitizenshipType citizenship, string? country, string? registration, City? earlyRegistration, string? placeOfBirth, string? documentAttached)
        {
            if (citizenship == CitizenshipType.RussianFederation)
            {
                country = "Российская Федерация";
            }

            CitizenshipInfo = new CitizenshipInfo(citizenship, country, registration, earlyRegistration, placeOfBirth, documentAttached);
        }

        /// <summary>
        /// Добавить документ
        /// </summary>
        /// <param name="type"></param>
        /// <param name="document"></param>
        /// <exception cref="ArgumentNullException">Если document равен null</exception>
        public void AddDocument(DocumentType type, Document document)
        {
            if (document is null)
                throw new ArgumentNullException(nameof(document), "Документ не может быть null");

            Documents[type] = document;
        }

        /// <summary>
        /// Изменить документ
        /// </summary>
        /// <param name="type"></param>
        /// <param name="newDocument"></param>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="ArgumentNullException">Если newDocument равен null</exception>
        public void UpdateDocument(DocumentType type, Document newDocument)
        {
            if (newDocument is null)
                throw new ArgumentNullException(nameof(newDocument), "Документ не может быть null");

            if (Documents.ContainsKey(type))
            {
                Documents[type] = newDocument;
            }
            else
            {
                throw new KeyNotFoundException($"Документ типа {type} не найден.");
            }
        }

        /// <summary>
        /// Добавить новую историю болезни
        /// </summary>
        /// <param name="medicalHistory">Новая история болезни</param>
        /// <exception cref="DomainException">Если уже есть активная история болезни</exception>
        public void AddMedicalHistory(MedicalHistory medicalHistory)
        {
            if (medicalHistory == null)
                throw new DomainException("История болезни не может быть null");

            if (ActiveHistory != null)
                throw new DomainException("Нельзя добавить новую историю болезни, пока не закрыта текущая");

            _medicalHistories.Add(medicalHistory);
        }

        /// <summary>s
        /// Закрыть активную историю болезни
        /// </summary>
        /// <param name="dischargeDate">Дата выписки</param>
        /// <exception cref="DomainException"></exception>
        public void CloseActiveHistory(DateTime dischargeDate)
        {
            var activeHistory = ActiveHistory;

            if (activeHistory == null)
                throw new DomainException("Нет активной истории болезни для закрытия");

            if (activeHistory.DateOfReceipt > dischargeDate)
                throw new DomainException("Дата выписки не может быть раньше, чем поступление в больницу");

            activeHistory.SetDateOfDischarge(dischargeDate);
        }

        /// <summary>
        /// Изменить статус дееспособного
        /// </summary>
        /// <param name="capable"></param>
        public void SetCapable(Capable? capable)
        {
            //todo: Сделать валидацию Capable
            Capable = capable;
        }

        //TODO: Изменить опекуна
        //TODO: Изменить распоряжение о назначение опекуна

        /// <summary>
        /// Изменить статус пенсионера
        /// </summary>
        /// <param name="pension"></param>
        public void SetPension(Pension? pension)
        {
            //todo: Сделать валидацию Pension 
            Pension = pension;
        }

        //TODO: Изменить группу инвалидности
        //TODO: Изменить с какого числа установлен статус пенсионера
        //TODO: Изменить способ получения пенсии
        //TODO: Изменить статус СФР
        //TODO: Изменить отделение СФР
        //TODO: Изменить РСД

        /// <summary>
        /// Изменить примечание
        /// </summary>
        /// <param name="note"></param>
        public void SetNote(string? note)
        {
            Note = note;
        }
    }
}
