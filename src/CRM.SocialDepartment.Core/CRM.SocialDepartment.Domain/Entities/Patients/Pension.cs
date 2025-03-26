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
        /// С какого числа установлен статус пенсионера
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
        private Pension() { }
        #pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.

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

        /// <summary>
        /// Изменить группу инвалидности
        /// </summary>
        /// <param name="disabilityGroup"></param>
        public void SetDisabilityGroup(DisabilityGroup disabilityGroup)
        {
            DisabilityGroup = disabilityGroup;
        }

        /// <summary>
        /// Изменить с какого числа установлен статус пенсионера
        /// </summary>
        /// <param name="pensionStartDateTime"></param>
        public void SetPensionStartDateTime(DateTime pensionStartDateTime)
        {
            PensionStartDateTime = pensionStartDateTime;
        }

        /// <summary>
        /// Изменить способ получения пенсии
        /// </summary>
        /// <param name="pensionAddress"></param>
        public void SetPensionAddress(PensionAddress pensionAddress)
        {
            PensionAddress = pensionAddress;
        }

        /// <summary>
        /// Изменить статус СФР
        /// </summary>
        /// <param name="sfrBranch"></param>
        public void SetSfrBranch(int sfrBranch)
        {
            SfrBranch = sfrBranch;
        }

        /// <summary>
        /// Изменить отделение СФР
        /// </summary>
        /// <param name="sfrDepartment"></param>
        public void SetSfrDepartment(string sfrDepartment)
        {
            SfrDepartment = sfrDepartment;
        }

        /// <summary>
        /// Изменить РСД
        /// </summary>
        /// <param name="rsd"></param>
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
