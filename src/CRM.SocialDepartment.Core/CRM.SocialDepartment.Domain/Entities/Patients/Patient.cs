using CRM.SocialDepartment.Domain.Entities.Patients.Documents;
using CRM.SocialDepartment.Domain.Events;
using CRM.SocialDepartment.Domain.Exceptions;
using DDD.Entities;

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

        private readonly List<MedicalHistory> _medicalHistories;

        /// <summary>
        /// Все истории болезни пациента
        /// </summary>
        public IReadOnlyList<MedicalHistory> MedicalHistories => _medicalHistories.AsReadOnly();

        /// <summary>
        /// Активная история болезни
        /// </summary>
        public MedicalHistory? ActiveHistory => _medicalHistories.FirstOrDefault(h => !h.DateOfDischarge.HasValue);

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

            // Генерируем событие создания пациента
            AddDomainEvent(new PatientCreatedEvent(this));
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
            
            // Генерируем событие обновления пациента
            AddDomainEvent(new PatientUpdatedEvent(this));
        }

        /// <summary>
        /// Изменить информацию о гражданстве
        /// </summary>
        /// <param name="citizenship">Гражданство</param>
        /// <param name="country">Страна</param>
        /// <param name="registration">Регистрация</param>
        /// <param name="earlyRegistration">Ранняя регистрация</param>
        /// <param name="placeOfBirth">Место рождения</param>
        /// <param name="documentAttached">Имеющиеся документы</param>
        public void ChangeCitizenshipInfo(CitizenshipType citizenship, string? country, string? registration, City? earlyRegistration, string? placeOfBirth, string? documentAttached)
        {
            if (citizenship == CitizenshipType.RussianFederation)
            {
                country = "Российская Федерация";
            }

            CitizenshipInfo = new CitizenshipInfo(citizenship, country, registration, earlyRegistration, placeOfBirth, documentAttached);
        }

        /// <summary>
        /// Изменить гражданство
        /// </summary>
        /// <param name="citizenship">Новое гражданство</param>
        public void SetCitizenship(CitizenshipType citizenship)
        {
            if (citizenship == CitizenshipType.RussianFederation)
            {
                CitizenshipInfo.SetCitizenship(citizenship).SetCountry("Российская Федерация");
            }
            else
            {
                CitizenshipInfo.SetCitizenship(citizenship);
            }
        }

        /// <summary>
        /// Изменить страну
        /// </summary>
        /// <param name="country">Новая страна</param>
        public void SetCountry(string? country)
        {
            CitizenshipInfo.SetCountry(country);
        }

        /// <summary>
        /// Изменить место регистрации
        /// </summary>
        /// <param name="registration">Новое место регистрации</param>
        public void SetRegistration(string? registration)
        {
            CitizenshipInfo.SetRegistration(registration);
        }

        /// <summary>
        /// Изменить раннюю регистрацию
        /// </summary>
        /// <param name="earlyRegistration">Новая ранняя регистрация</param>
        public void SetEarlyRegistration(City? earlyRegistration)
        {
            CitizenshipInfo.SetEarlyRegistration(earlyRegistration);
        }

        /// <summary>
        /// Изменить место рождения
        /// </summary>
        /// <param name="placeOfBirth">Новое место рождения</param>
        public void SetPlaceOfBirth(string? placeOfBirth)
        {
            CitizenshipInfo.SetPlaceOfBirth(placeOfBirth);
        }

        /// <summary>
        /// Изменить имеющиеся документы
        /// </summary>
        /// <param name="documentAttached">Новые имеющиеся документы</param>
        public void SetDocumentAttached(string? documentAttached)
        {
            CitizenshipInfo.SetDocumentAttached(documentAttached);
        }

        /// <summary>
        /// Добавляет документ пациенту
        /// </summary>
        /// <param name="document">Документ для добавления</param>
        /// <exception cref="DomainException">Если документ такого типа уже существует</exception>
        public void AddDocument(Document document)
        {
            if (document is null)
                throw new ArgumentNullException(nameof(document), "Документ не может быть null");

            var documentType = GetDocumentType(document);
            if (Documents.ContainsKey(documentType))
                throw new DomainException($"Документ типа {document.DisplayName} уже существует");

            Documents[documentType] = document;
            
            // Генерируем событие добавления документа
            AddDomainEvent(new PatientDocumentAddedEvent(Id, documentType.ToString(), document.Number));
        }

        /// <summary>
        /// Обновляет существующий документ пациента
        /// </summary>
        /// <param name="document">Новая версия документа</param>
        /// <exception cref="DomainException">Если документ такого типа не существует</exception>
        public void UpdateDocument(Document document)
        {
            if (document is null)
                throw new ArgumentNullException(nameof(document), "Документ не может быть null");

            var documentType = GetDocumentType(document);
            if (!Documents.ContainsKey(documentType))
                throw new DomainException($"Документ типа {document.DisplayName} не найден");

            Documents[documentType] = document;
        }

        /// <summary>
        /// Определяет тип документа на основе его типа
        /// </summary>
        private static DocumentType GetDocumentType(Document document)
        {
            return document switch
            {
                PassportDocument => DocumentType.Passport,
                MedicalPolicyDocument => DocumentType.MedicalPolicy,
                SnilsDocument => DocumentType.Snils,
                _ => throw new DomainException($"Неизвестный тип документа: {document.GetType().Name}")
            };
        }

        /// <summary>
        /// Удаляет документ пациента
        /// </summary>
        /// <param name="documentType">Тип документа для удаления</param>
        /// <exception cref="DomainException">Если документ такого типа не существует</exception>
        public void RemoveDocument(DocumentType documentType)
        {
            if (!Documents.ContainsKey(documentType))
                throw new DomainException($"Документ типа {documentType} не найден");

            Documents.Remove(documentType);
        }

        /// <summary>
        /// Проверяет наличие обязательных документов
        /// </summary>
        /// <returns>True если все обязательные документы присутствуют</returns>
        public bool HasRequiredDocuments()
        {
            return Documents.ContainsKey(DocumentType.Passport) &&
                   Documents.ContainsKey(DocumentType.MedicalPolicy) &&
                   Documents.ContainsKey(DocumentType.Snils);
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

        /// <summary>
        /// Изменить номер отделения в активной истории болезни
        /// </summary>
        /// <param name="numberDepartment">Новый номер отделения</param>
        /// <exception cref="DomainException">Если нет активной истории болезни</exception>
        public void SetMedicalHistoryDepartment(sbyte numberDepartment)
        {
            if (ActiveHistory is null)
                throw new DomainException("Нет активной истории болезни");

            ((IMedicalHistoryInternal)ActiveHistory).SetNumberDepartment(numberDepartment);
        }

        /// <summary>
        /// Закрыть активную историю болезни
        /// </summary>
        /// <param name="dischargeDate">Дата выписки</param>
        /// <exception cref="DomainException">Если нет активной истории болезни или дата выписки раньше даты поступления</exception>
        public void CloseActiveHistory(DateTime dischargeDate)
        {
            if (ActiveHistory is null)
                throw new DomainException("Нет активной истории болезни");

            if (ActiveHistory.DateOfReceipt > dischargeDate)
                throw new DomainException("Дата выписки не может быть раньше, чем поступление в больницу");

            ((IMedicalHistoryInternal)ActiveHistory).SetDateOfDischarge(dischargeDate);
        }

        /// <summary>
        /// Изменить статус дееспособного
        /// </summary>
        /// <param name="capable">Новая информация о дееспособности</param>
        public void SetCapable(Capable? capable)
        {
            //TODO: Сделать валидацию Capable
            Capable = capable;
        }

        /// <summary>
        /// Изменить опекуна
        /// </summary>
        /// <param name="guardian">Новый опекун</param>
        /// <exception cref="DomainException">Если пациент дееспособен</exception>
        public void SetGuardian(string guardian)
        {
            if (Capable is null)
                throw new DomainException("Пациент дееспособен");

            ((ICapableInternal)Capable).SetGuardian(guardian);
        }

        /// <summary>
        /// Изменить распоряжение о назначении опекуна
        /// </summary>
        /// <param name="guardianOrderAppointment">Новое распоряжение о назначении опекуна</param>
        /// <exception cref="DomainException">Если пациент дееспособен</exception>
        public void SetGuardianOrderAppointment(string guardianOrderAppointment)
        {
            if (Capable is null)
                throw new DomainException("Пациент дееспособен");

            ((ICapableInternal)Capable).SetGuardianOrderAppointment(guardianOrderAppointment);
        }

        /// <summary>
        /// Изменить статус пенсионера
        /// </summary>
        /// <param name="pension"></param>
        public void SetPension(Pension? pension)
        {
            //TODO: Сделать валидацию Pension 
            Pension = pension;
        }

        /// <summary>
        /// Изменить группу инвалидности
        /// </summary>
        /// <param name="disabilityGroup">Новая группа инвалидности</param>
        /// <exception cref="DomainException">Если пациент не получает пенсию</exception>
        public void SetDisabilityGroup(DisabilityGroup disabilityGroup)
        {
            if (Pension is null)
                throw new DomainException("Пациент не получает пенсию");

            ((IPensionInternal)Pension).SetDisabilityGroup(disabilityGroup);
        }

        /// <summary>
        /// Изменить дату установления статуса пенсионера
        /// </summary>
        /// <param name="pensionStartDateTime">Новая дата установления статуса</param>
        /// <exception cref="DomainException">Если пациент не получает пенсию</exception>
        public void SetPensionStartDateTime(DateTime pensionStartDateTime)
        {
            if (Pension is null)
                throw new DomainException("Пациент не получает пенсию");

            ((IPensionInternal)Pension).SetPensionStartDateTime(pensionStartDateTime);
        }

        /// <summary>
        /// Изменить способ получения пенсии
        /// </summary>
        /// <param name="pensionAddress">Новый способ получения пенсии</param>
        /// <exception cref="DomainException">Если пациент не получает пенсию</exception>
        public void SetPensionAddress(PensionAddress pensionAddress)
        {
            if (Pension is null)
                throw new DomainException("Пациент не получает пенсию");

            ((IPensionInternal)Pension).SetPensionAddress(pensionAddress);
        }

        /// <summary>
        /// Изменить филиал СФР
        /// </summary>
        /// <param name="sfrBranch">Новый филиал СФР</param>
        /// <exception cref="DomainException">Если пациент не получает пенсию</exception>
        public void SetSfrBranch(int sfrBranch)
        {
            if (Pension is null)
                throw new DomainException("Пациент не получает пенсию");

            ((IPensionInternal)Pension).SetSfrBranch(sfrBranch);
        }

        /// <summary>
        /// Изменить отделение СФР
        /// </summary>
        /// <param name="sfrDepartment">Новое отделение СФР</param>
        /// <exception cref="DomainException">Если пациент не получает пенсию</exception>
        public void SetSfrDepartment(string sfrDepartment)
        {
            if (Pension is null)
                throw new DomainException("Пациент не получает пенсию");

            ((IPensionInternal)Pension).SetSfrDepartment(sfrDepartment);
        }

        /// <summary>
        /// Изменить РСД
        /// </summary>
        /// <param name="rsd">Новый РСД</param>
        /// <exception cref="DomainException">Если пациент не получает пенсию</exception>
        public void SetRsd(string? rsd)
        {
            if (Pension is null)
                throw new DomainException("Пациент не получает пенсию");

            ((IPensionInternal)Pension).SetRsd(rsd);
        }

        /// <summary>
        /// Изменить примечание
        /// </summary>
        /// <param name="note"></param>
        public void SetNote(string? note)
        {
            Note = note;
            
            // Генерируем событие обновления пациента
            AddDomainEvent(new PatientUpdatedEvent(this));
        }

        /// <summary>
        /// Архивировать пациента (выписка)
        /// </summary>
        /// <param name="reason">Причина архивирования</param>
        public void Archive(string? reason = null)
        {
            if (IsArchive)
                throw new DomainException("Пациент уже находится в архиве");

            IsArchive = true;
            
            // Генерируем событие архивирования пациента
            AddDomainEvent(new PatientArchivedEvent(this, reason));
        }

        /// <summary>
        /// Восстановить пациента из архива
        /// </summary>
        public void Unarchive()
        {
            if (!IsArchive)
                throw new DomainException("Пациент не находится в архиве");

            IsArchive = false;
            
            // Генерируем событие восстановления пациента
            AddDomainEvent(new PatientUnarchivedEvent(this));
        }

        /// <summary>
        /// Пометить пациента как удаленного (мягкое удаление)
        /// </summary>
        public void SoftDelete()
        {
            if (SoftDeleted)
                throw new DomainException("Пациент уже помечен как удаленный");

            SoftDeleted = true;
            
            // Генерируем событие удаления пациента
            AddDomainEvent(new PatientDeletedEvent(Id, FullName));
        }
    }
}
