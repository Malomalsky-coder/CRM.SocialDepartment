using CRM.SocialDepartment.Domain.Entities.Patients;

namespace CRM.SocialDepartment.Domain.Specifications
{
    public class DisabilityGroupWithoutPeriodSpecification : ISpecification<DisabilityGroup>
    {
        public bool IsSatisfiedBy(DisabilityGroup group)
        {
            return group == DisabilityGroup.OneGroupWithoutPeriod ||
                   group == DisabilityGroup.TwoGroupWithoutPeriod ||
                   group == DisabilityGroup.ThreeGroupWithoutPeriod;
        }
    }
}
