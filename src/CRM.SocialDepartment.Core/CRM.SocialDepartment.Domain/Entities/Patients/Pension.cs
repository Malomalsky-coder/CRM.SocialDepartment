using DDD.Values;

namespace CRM.SocialDepartment.Domain.Entities.Patients
{
    public class Pension : ValueObject
    {
        /// <summary>
        /// Группа инвалидности
        /// </summary>
        public DisabilityGroup DisabilityGroup { get; private set; }

        /// <summary>
        /// С какого числа бессрочная группа
        /// </summary>
        public DateTime? PensionStartDateTime { get; private set; }

        /// <summary>
        /// Способ получения пенсии
        /// </summary>
        public PensionAddress PensionAddress { get; private set; }

        /// <summary>
        /// Филиал СФР
        /// </summary>
        public int SfrBranch { get; private set; }

        /// <summary>
        /// Отделение СФР
        /// </summary>
        public string SfrDepartment { get; private set; }

        /// <summary>
        /// РСД
        /// </summary>
        public string? Rsd { get; set; }


#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        private Pension()
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        {
            
        }

        public Pension(
            DisabilityGroup disabilityGroup,
            DateTime pensionStartDateTime,
            PensionAddress pensionAddress,
            int sfrBranch,
            string sfrDepartment,
            string? rsd)
        {
            DisabilityGroup = disabilityGroup;
            PensionStartDateTime = pensionStartDateTime;
            PensionAddress = pensionAddress;
            SfrBranch = sfrBranch;
            SfrDepartment = sfrDepartment;
            Rsd = rsd;
        }

        public void SetDisabilityGroup(DisabilityGroup disabilityGroup)
        {
            DisabilityGroup = disabilityGroup;
        }

        public void SetPensionStartDateTime(DateTime pensionStartDateTime)
        {
            PensionStartDateTime = pensionStartDateTime;
        }

        public void SetPensionAddress(PensionAddress pensionAddress)
        {
            PensionAddress = pensionAddress;
        }

        public void SetSfrBranch(int sfrBranch)
        {
            SfrBranch = sfrBranch;
        }

        public void SetSfrDepartment(string sfrDepartment)
        {
            SfrDepartment = sfrDepartment;
        }

        public void SetRsd(string? rsd)
        {
            Rsd = rsd;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return DisabilityGroup;
            yield return PensionStartDateTime ?? default;
            yield return PensionAddress;
            yield return SfrBranch;
            yield return SfrDepartment;
            yield return Rsd ?? string.Empty;
        }
    }
}
