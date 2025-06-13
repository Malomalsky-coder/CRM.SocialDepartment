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
                new SelectListItem { Value = "0", Text = PensionAddress.None.DisplayName },
                new SelectListItem { Value = "1", Text = PensionAddress.PHC5.DisplayName },
                new SelectListItem { Value = "2", Text = PensionAddress.OSB.DisplayName },
                new SelectListItem { Value = "3", Text = PensionAddress.Registration.DisplayName }
            ];
        }
    }
}
