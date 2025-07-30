using CRM.SocialDepartment.Domain.Entities.Patients;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CRM.SocialDepartment.Site.Extensions
{
    public class PensionAddressExtensions
    {
        public static IEnumerable<SelectListItem> ToSelectList()
        {
            return
            [
                new SelectListItem { Value = "0", Text = PensionAddressType.None.DisplayName },
                new SelectListItem { Value = "1", Text = PensionAddressType.PHC5.DisplayName },
                new SelectListItem { Value = "2", Text = PensionAddressType.OSB.DisplayName },
                new SelectListItem { Value = "3", Text = PensionAddressType.Registration.DisplayName }
            ];
        }
    }
}
