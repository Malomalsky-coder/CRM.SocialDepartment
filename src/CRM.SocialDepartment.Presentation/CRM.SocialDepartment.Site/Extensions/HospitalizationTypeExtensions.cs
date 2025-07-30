using CRM.SocialDepartment.Domain.Entities.Patients;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CRM.SocialDepartment.Site.Extensions
{
    public static class HospitalizationTypeExtensions
    {
        public static IEnumerable<SelectListItem> ToSelectList()
        {
            return
            [
                new SelectListItem { Value = "1", Text = HospitalizationType.Force.DisplayName },
                new SelectListItem { Value = "2", Text = HospitalizationType.Voluntary.DisplayName },
                new SelectListItem { Value = "3", Text = HospitalizationType.CriminalCodeRFArticle435.DisplayName }
            ];
        }
    }
}
