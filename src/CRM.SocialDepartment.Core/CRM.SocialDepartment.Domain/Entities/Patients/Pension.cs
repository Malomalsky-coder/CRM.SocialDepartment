using DDD.Values;

namespace CRM.SocialDepartment.Domain.Entities.Patients
{
    /// <summary>
    /// Публичный интерфейс для пенсии
    /// </summary>
    public interface IPension
    {
        /// <summary>
        /// Группа инвалидности
        /// </summary>
        DisabilityGroupType DisabilityGroup { get; }

        /// <summary>
        /// Дата установления статуса пенсионера
        /// </summary>
        DateTime? PensionStartDateTime { get; }

        /// <summary>
        /// Способ получения пенсии
        /// </summary>
        PensionAddressType PensionAddress { get; }

        /// <summary>
        /// Филиал СФР
        /// </summary>
        int SfrBranch { get; }

        /// <summary>
        /// Отделение СФР
        /// </summary>
        string SfrDepartment { get; }

        /// <summary>
        /// РСД
        /// </summary>
        string? Rsd { get; }
    }

    /// <summary>
    /// Внутренний интерфейс для пенсии
    /// </summary>
    internal interface IPensionInternal : IPension
    {
        void SetDisabilityGroup(DisabilityGroupType disabilityGroup);
        void SetPensionStartDateTime(DateTime pensionStartDateTime);
        void SetPensionAddress(PensionAddressType pensionAddress);
        void SetSfrBranch(int sfrBranch);
        void SetSfrDepartment(string sfrDepartment);
        void SetRsd(string? rsd);
    }

    /// <summary>
    /// Пенсия
    /// </summary>
    public class Pension : ValueObject, IPensionInternal
    {
        /// <summary>
        /// Группа инвалидности
        /// </summary>
        public DisabilityGroupType DisabilityGroup { get; private set; }

        /// <summary>
        /// Дата установления статуса пенсионера
        /// </summary>
        public DateTime? PensionStartDateTime { get; private set; }

        /// <summary>
        /// Способ получения пенсии
        /// </summary>
        public PensionAddressType PensionAddress { get; private set; }

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
        public string? Rsd { get; private set; }

        #pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        private Pension() { }
        #pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.

        public Pension(
            DisabilityGroupType disabilityGroup,
            DateTime? pensionStartDateTime,
            PensionAddressType pensionAddress,
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
        /// <param name="disabilityGroup">Новая группа инвалидности</param>
        void IPensionInternal.SetDisabilityGroup(DisabilityGroupType disabilityGroup)
        {
            DisabilityGroup = disabilityGroup;
        }

        /// <summary>
        /// Изменить дату установления статуса пенсионера
        /// </summary>
        /// <param name="pensionStartDateTime">Новая дата установления статуса</param>
        void IPensionInternal.SetPensionStartDateTime(DateTime pensionStartDateTime)
        {
            PensionStartDateTime = pensionStartDateTime;
        }

        /// <summary>
        /// Изменить способ получения пенсии
        /// </summary>
        /// <param name="pensionAddress">Новый способ получения пенсии</param>
        void IPensionInternal.SetPensionAddress(PensionAddressType pensionAddress)
        {
            PensionAddress = pensionAddress;
        }

        /// <summary>
        /// Изменить филиал СФР
        /// </summary>
        /// <param name="sfrBranch">Новый филиал СФР</param>
        void IPensionInternal.SetSfrBranch(int sfrBranch)
        {
            SfrBranch = sfrBranch;
        }

        /// <summary>
        /// Изменить отделение СФР
        /// </summary>
        /// <param name="sfrDepartment">Новое отделение СФР</param>
        void IPensionInternal.SetSfrDepartment(string sfrDepartment)
        {
            SfrDepartment = sfrDepartment;
        }

        /// <summary>
        /// Изменить РСД
        /// </summary>
        /// <param name="rsd">Новый РСД</param>
        void IPensionInternal.SetRsd(string? rsd)
        {
            Rsd = rsd;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return DisabilityGroup;
            yield return PensionStartDateTime!.Value;
            yield return PensionAddress;
            yield return SfrBranch;
            yield return SfrDepartment;
            yield return Rsd ?? string.Empty;
        }
    }
}
