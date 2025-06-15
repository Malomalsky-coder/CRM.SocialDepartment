using DDD.Values;

namespace CRM.SocialDepartment.Domain.Entities.Patients
{
    /// <summary>
    /// Публичный интерфейс для дееспособности
    /// </summary>
    public interface ICapable
    {
        /// <summary>
        /// Решение суда
        /// </summary>
        string CourtDecision { get; }

        /// <summary>
        /// Дата суда
        /// </summary>
        DateTime TrialDate { get; }

        /// <summary>
        /// Опекун
        /// </summary>
        string Guardian { get; }

        /// <summary>
        /// Распоряжение о назначении опекуна
        /// </summary>
        string GuardianOrderAppointment { get; }
    }

    /// <summary>
    /// Внутренний интерфейс для дееспособности
    /// </summary>
    internal interface ICapableInternal : ICapable
    {
        void SetGuardian(string guardian);
        void SetGuardianOrderAppointment(string guardianOrderAppointment);
    }

    /// <summary>
    /// Дееспособный
    /// </summary>
    public class Capable : ValueObject, ICapableInternal
    {
        /// <summary>
        /// Решение суда
        /// </summary>
        public string CourtDecision { get; private set; }

        /// <summary>
        /// Дата суда
        /// </summary>
        public DateTime TrialDate { get; private set; }

        /// <summary>
        /// Опекун
        /// </summary>
        public string Guardian { get; private set; }

        /// <summary>
        /// Распоряжение о назначении опекуна
        /// </summary>
        public string GuardianOrderAppointment { get; private set; }

        #pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        private Capable() { }
        #pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.

        public Capable(
            string courtDecision,
            DateTime trialDate,
            string guardian,
            string guardianOrderAppointment)
        {
            CourtDecision = courtDecision;
            TrialDate = trialDate;
            Guardian = guardian;
            GuardianOrderAppointment = guardianOrderAppointment;
        }

        /// <summary>
        /// Изменить опекуна
        /// </summary>
        /// <param name="guardian">Новый опекун</param>
        void ICapableInternal.SetGuardian(string guardian)
        {
            Guardian = guardian;
        }

        /// <summary>
        /// Изменить распоряжение о назначении опекуна
        /// </summary>
        /// <param name="guardianOrderAppointment">Новое распоряжение о назначении опекуна</param>
        void ICapableInternal.SetGuardianOrderAppointment(string guardianOrderAppointment)
        {
            GuardianOrderAppointment = guardianOrderAppointment;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return CourtDecision;
            yield return TrialDate;
            yield return Guardian;
            yield return GuardianOrderAppointment;
        }
    }
}
