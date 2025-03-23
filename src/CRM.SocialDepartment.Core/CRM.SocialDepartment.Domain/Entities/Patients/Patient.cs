using DDD.Entities;

namespace CRM.SocialDepartment.Domain.Entities.Patients
{
    public class Patient : AggregateRoot<Guid>, IArchive, ISoftDelete
    {
        //todo: номер отделения, поместить в историю болезни?

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
        /// Гражданство
        /// </summary>
        public string Citizenship { get; private set; }

        /// <summary>
        /// Страна
        /// </summary>
        public string Country { get; private set; }

        /// <summary>
        /// Место регистрации
        /// </summary>
        public string? Registration { get; private set; }

        /// <summary>
        /// Нет регистрации, бомж
        /// </summary>
        public bool NotRegistered => Registration is null;

        /// <summary>
        /// Ранняя регистрация
        /// </summary>
        public City? EarlyRegistration { get; private set; }

        /// <summary>
        /// Место рождения
        /// </summary>
        public string? PlaceOfBirth { get; private set; }

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
        /// Помечен как удаленный
        /// </summary>
        public bool SoftDeleted { get; set; }

        /// <summary>
        /// Помечен как в архиве (пациент выписан)
        /// </summary>
        public bool IsArchive { get; set; }


#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        private Patient()
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        {
            
        }

        public Patient(
            string fullname,
            DateTime birthday,
            string citizenship,
            string country,
            string? registration,
            City? earlyRegistration,
            string? placeOfBirth,
            Capable? capable,
            Pension? pension,
            string? note
            )
        {
            FullName = fullname;
            Birthday = birthday;
            Citizenship = citizenship;
            Country = country;
            Registration = registration;
            EarlyRegistration = earlyRegistration;
            PlaceOfBirth = placeOfBirth;
            Documents = new Dictionary<Document, object>();
            Capable = capable;
            Pension = pension;
            Note = note;
        }

        public void SetFullName(string fullName)
        {
            FullName = fullName;
        }

        public void SetBirthday(DateTime birthday)
        {
            Birthday = birthday;
        }

        public void SetCitizenship(string citizenship)
        {
            Citizenship = citizenship;
        }

        public void SetCountry(string country)
        {
            Country = country;
        }

        public void SetRegistration(string? registration)
        {
            Registration = registration;
        }

        public void SetEarlyRegistration(City? earlyRegistration)
        {
            EarlyRegistration = earlyRegistration;
        }

        public void SetPlaceOfBirth(string? placeOfBirth)
        {
            PlaceOfBirth = placeOfBirth;
        }

        public void AddDocument(Document type, object document)
        {
            Documents[type] = document;
        }

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

        public void SetCapable(Capable? capable)
        {
            Capable = capable;
        }

        public void SetPension(Pension? pension)
        {
            Pension = pension;
        }

        public void SetNote(string? note)
        {
            Note = note;
        }
    }
}
