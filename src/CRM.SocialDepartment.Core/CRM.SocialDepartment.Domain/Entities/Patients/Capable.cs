﻿using DDD.Values;
using MongoDB.Bson.Serialization.Attributes;

namespace CRM.SocialDepartment.Domain.Entities.Patients
{
    /// <summary>
    /// Дееспособный
    /// </summary>
    public class Capable : ValueObject
    {
        /// <summary>
        /// Решение суда
        /// </summary>
        public string CourtDecision { get; private set; }

        /// <summary>
        /// Дата проведения суда
        /// </summary>
        public DateTime TrialDate { get; private set; }

        /// <summary>
        /// Опекун
        /// </summary>
        public string Guardian { get; private set; }

        /// <summary>
        /// Распоряжение о назначение опекуна
        /// </summary>
        public string GuardianOrderAppointment { get; private set; }

        #pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        private Capable() { }
        #pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.

        public Capable(string courtDecision, DateTime trialDate, string guardian, string guardianOrderAppointment)
        {
            CourtDecision = courtDecision;
            TrialDate = trialDate;
            Guardian = guardian;
            GuardianOrderAppointment = guardianOrderAppointment;
        }

        /// <summary>
        /// Изменить опекуна
        /// </summary>
        /// <param name="guardian"></param>
        public void SetGuardian(string guardian)
        {
            Guardian = guardian;
        }

        /// <summary>
        /// Изменить распоряжение о назначение опекуна
        /// </summary>
        /// <param name="guardianOrderAppointment"></param>
        public void SetGuardianOrderAppointment(string guardianOrderAppointment)
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
