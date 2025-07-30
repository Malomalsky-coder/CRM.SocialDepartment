using CRM.SocialDepartment.Domain.Entities.Patients;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CRM.SocialDepartment.Site.Extensions
{
    public class DisabilityGroupExtensions
    {
        public static IEnumerable<SelectListItem> ToSelectList()
        {
            return
            [
                new SelectListItem { Value = "0", Text = DisabilityGroupType.None.DisplayName },
                new SelectListItem { Value = "1", Text = DisabilityGroupType.OneGroup.DisplayName },
                new SelectListItem { Value = "2", Text = DisabilityGroupType.OneGroupWithoutPeriod.DisplayName },
                new SelectListItem { Value = "3", Text = DisabilityGroupType.TwoGroup.DisplayName },
                new SelectListItem { Value = "4", Text = DisabilityGroupType.TwoGroupWithoutPeriod.DisplayName },
                new SelectListItem { Value = "5", Text = DisabilityGroupType.ThreeGroup.DisplayName },
                new SelectListItem { Value = "6", Text = DisabilityGroupType.ThreeGroupWithoutPeriod.DisplayName },
                new SelectListItem { Value = "7", Text = DisabilityGroupType.ChildrenDisable.DisplayName }
            ];
        }
    }
}
