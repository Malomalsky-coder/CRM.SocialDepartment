using CRM.SocialDepartment.Domain.Entities.Patients;

namespace CRM.SocialDepartment.Domain.Specifications
{
    public class DisabilityGroupWithoutPeriodSpecification : ISpecification<DisabilityGroupType>
    {
        public bool IsSatisfiedBy(DisabilityGroupType group)
        {
            return group == DisabilityGroupType.OneGroupWithoutPeriod ||
                   group == DisabilityGroupType.TwoGroupWithoutPeriod ||
                   group == DisabilityGroupType.ThreeGroupWithoutPeriod;
        }
    }
}
