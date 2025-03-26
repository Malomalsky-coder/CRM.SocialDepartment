using System.ComponentModel.DataAnnotations;

namespace CRM.SocialDepartment.Domain.Entities.Patients
{
    public enum CitizenshipType : byte
    {
        [Display(Name = "Российская Федерация")]
        RussianFederation,

        [Display(Name = "Иностранец")]
        Foreigner,

        [Display(Name = "ЛБГ")]
        StatelessPerson
    }
}
