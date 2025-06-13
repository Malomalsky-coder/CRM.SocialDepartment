using CRM.SocialDepartment.Domain.Entities.Patients;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CRM.SocialDepartment.Site.Extensions
{
    public static class CitizenshipTypeExtensions
    {
        public static IEnumerable<SelectListItem> ToSelectList()
        {
            return new[]
            {
            new SelectListItem { Value = "0", Text = CitizenshipType.RussianFederation.DisplayName },
            new SelectListItem { Value = "1", Text = CitizenshipType.Foreigner.DisplayName },
            new SelectListItem { Value = "2", Text = CitizenshipType.StatelessPerson.DisplayName }
        };
        }
    }
}
