using CRM.SocialDepartment.Domain.Entities.Patients;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CRM.SocialDepartment.Site.Extensions
{
    public static class CityExtensions
    {
        public static IEnumerable<SelectListItem> ToSelectList()
        {
            return
            [
                new SelectListItem { Value = "0", Text = City.None.DisplayName },
                new SelectListItem { Value = "1", Text = City.Moscow.DisplayName },
                new SelectListItem { Value = "2", Text = City.FromAnotherTown.DisplayName }
            ];
        }
    }
}
