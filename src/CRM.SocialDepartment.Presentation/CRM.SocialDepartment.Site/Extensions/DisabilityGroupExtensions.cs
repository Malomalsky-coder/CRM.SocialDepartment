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
                new SelectListItem { Value = "0", Text = DisabilityGroup.None.DisplayName },
                new SelectListItem { Value = "1", Text = DisabilityGroup.OneGroup.DisplayName },
                new SelectListItem { Value = "2", Text = DisabilityGroup.OneGroupWithoutPeriod.DisplayName },
                new SelectListItem { Value = "3", Text = DisabilityGroup.TwoGroup.DisplayName },
                new SelectListItem { Value = "4", Text = DisabilityGroup.TwoGroupWithoutPeriod.DisplayName },
                new SelectListItem { Value = "5", Text = DisabilityGroup.ThreeGroup.DisplayName },
                new SelectListItem { Value = "6", Text = DisabilityGroup.ThreeGroupWithoutPeriod.DisplayName },
                new SelectListItem { Value = "7", Text = DisabilityGroup.ChildrenDisable.DisplayName }
            ];
        }
    }
}
