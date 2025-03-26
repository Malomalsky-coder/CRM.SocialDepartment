using AutoMapper;
using CRM.SocialDepartment.Application.DTOs;
using CRM.SocialDepartment.Domain.Entities.Patients;

namespace CRM.SocialDepartment.Site.MappingProfile
{
    public class ProjectMappingProfile : Profile
    {
        public ProjectMappingProfile()
        {
            // Patient ///////////////////////////////////////////////////////
            CreateMap<Patient, CreateOrEditPatientDTO>();
            CreateMap<CapableDTO, Capable>()
                .ConstructUsing(src => new Capable(src.CourtDecision, src.TrialDate, src.Guardian, src.GuardianOrderAppointment));

            CreateMap<PensionDTO, Pension>();
        }
    }
}