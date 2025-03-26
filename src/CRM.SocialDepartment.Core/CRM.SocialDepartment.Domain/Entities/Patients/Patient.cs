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

        /// <summary>
        /// Информация о гражданстве
        /// </summary>
        public CitizenshipInfo CitizenshipInfo { get; private set; }

        /// <summary>
        /// Список документов
        /// </summary>
        public Dictionary<Document, object> Documents { get; private set; }

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
        /// Истории болезней
        /// </summary>
        //public List<Guid> HistoryOfIllnessIds { get; private set; }

        /// <summary>
        /// Помечен как удаленный
        /// </summary>
        public bool SoftDeleted { get; set; }

        /// <summary>
        /// Помечен как в архиве (пациент выписан)
        /// </summary>
        public bool IsArchive { get; set; }

        #pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        private Patient() { }
        #pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.

        public Patient(
            Guid id,
            string fullname,
            DateTime birthday,
            CitizenshipInfo citizenshipInfo,
            Capable? capable,
            Pension? pension,
            string? note
            )
        {
            Id = id;
            FullName = fullname;
            Birthday = birthday;
            CitizenshipInfo = citizenshipInfo;
            Documents = new Dictionary<Document, object>();
            Capable = capable;
            Pension = pension;
            Note = note;
        }

        /// <summary>
        /// Изменить фио
        /// </summary>
        /// <param name="fullName"></param>
        public void ChangeFullName(string fullName)
        {
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
        public void ChangeCitizenshipInfo(CitizenshipType citizenship, string? country, string? registration, City? earlyRegistration, string? placeOfBirth)
        {
            if (citizenship == CitizenshipType.RussianFederation)
            {
                country = "Российская Федерация";
            }

            CitizenshipInfo = new CitizenshipInfo(citizenship, country, registration, earlyRegistration, placeOfBirth);
        }

        /// <summary>
        /// Добавить документ
        /// </summary>
        /// <param name="type"></param>
        /// <param name="document"></param>
        public void AddDocument(Document type, object document)
        {
            Documents[type] = document;
        }

        /// <summary>
        /// Изменить документ
        /// </summary>
        /// <param name="type"></param>
        /// <param name="newDocument"></param>
        /// <exception cref="KeyNotFoundException"></exception>
        public void UpdateDocument(Document type, object newDocument)
        {
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
        /// Изменить статус дееспособного
        /// </summary>
        /// <param name="capable"></param>
        public void SetCapable(Capable? capable)
        {
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
