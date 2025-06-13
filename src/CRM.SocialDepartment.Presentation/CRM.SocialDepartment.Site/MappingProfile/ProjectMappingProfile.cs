using AutoMapper;
using CRM.SocialDepartment.Application.DTOs;
using CRM.SocialDepartment.Domain.Entities;
using CRM.SocialDepartment.Domain.Entities.Patients;
using CRM.SocialDepartment.Domain.Entities.Patients.Factories;
using CRM.SocialDepartment.Site.ViewModels.Patient;

namespace CRM.SocialDepartment.Site.MappingProfile
{
    public class ProjectMappingProfile : Profile
    {
        public ProjectMappingProfile()
        {
            // Patient //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            CreateMap<CreatePatientViewModel, CreatePatientDTO>()
                .ForMember(dest => dest.Documents, opt => opt.MapFrom(src =>
                    src.Documents.ToDictionary(
                        kvp => kvp.Key,
                        kvp => new DocumentDTO
                        {
                            Type = kvp.Key,
                            Number = kvp.Value.Number
                        }
                    )));

            CreateMap<EditPatientViewModel, EditPatientDTO>()
                .ForMember(dest => dest.Documents, opt => opt.MapFrom(src =>
                    src.Documents.ToDictionary(
                        kvp => kvp.Key,
                        kvp => new DocumentDTO
                        {
                            Type = kvp.Key,
                            Number = kvp.Value.Number
                        }
                    )));

            CreateMap<Patient, EditPatientDTO>()
                .ForMember(dest => dest.Documents, opt => opt.MapFrom(src => 
                    src.Documents.ToDictionary(
                        kvp => kvp.Key,
                        kvp => new DocumentDTO
                        {
                            Type = kvp.Key,
                            Number = kvp.Value.Number
                        }
                    )));

            //По лучшим практикам, процесс маппинга из DTO -> Entity, должен осуществляться в ручную.
            //CreateMap<EditPatientDTO, Patient>()
            //.ForMember(dest => dest.Documents, opt => opt.MapFrom(src => 
            //    src.Documents.ToDictionary(
            //        kvp => kvp.Key,
            //        kvp => DocumentFactory.Create(
            //            kvp.Value.Type,
            //            kvp.Value.Number
            //        )
            //    )));

            CreateMap<Patient, CreatePatientDTO>()
                .ForMember(dest => dest.Documents, opt => opt.MapFrom(src => 
                    src.Documents.ToDictionary(
                        kvp => kvp.Key,
                        kvp => new DocumentDTO
                        {
                            Type = kvp.Key,
                            Number = kvp.Value.Number
                        }
                    )));

            //По лучшим практикам, процесс маппинга из DTO -> Entity, должен осуществляться в ручную.
            //CreateMap<CreatePatientDTO, Patient>()
            //.ForMember(dest => dest.Documents, opt => opt.MapFrom(src => 
            //    src.Documents.ToDictionary(
            //        kvp => kvp.Key,
            //        kvp => DocumentFactory.Create(
            //            kvp.Value.Type,
            //            kvp.Value.Number
            //        )
            //    )));

            CreateMap<CreatePatientViewModel, CreatePatientDTO>();
            CreateMap<EditPatientViewModel, EditPatientDTO>();

            // HospitalizationType //////////////////////////////////////////////////////////////////////////////////////////////////////
            CreateMap<byte, HospitalizationType>().ConvertUsing(value => HospitalizationType.FromValue(value));
            CreateMap<HospitalizationType, byte>().ConvertUsing(hospitalizationType => hospitalizationType.Value);

            // CitizenshipInfo ///////////////////////////////////////////////////////////////////////////////////////////////////////////
            CreateMap<CitizenshipInfoDTO, Domain.Entities.Patients.CitizenshipInfo>().ReverseMap();

            // CitizenshipType ///////////////////////////////////////////////////////////////////////////////////////////////////////////
            CreateMap<byte, CitizenshipType>().ConvertUsing(value => CitizenshipType.FromValue(value));
            CreateMap<CitizenshipType, byte>().ConvertUsing(citizenshipType => citizenshipType.Value);

            // Capable //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            CreateMap<CreateCapableDTO, Domain.Entities.Patients.Capable>()
                .ConstructUsing(src => new Domain.Entities.Patients.Capable(
                    src.CourtDecision,
                    src.TrialDate,
                    src.Guardian,
                    src.GuardianOrderAppointment));

            CreateMap<EditCapableDTO, Domain.Entities.Patients.Capable>()
                .ForMember(dest => dest.CourtDecision, opt => opt.Ignore())
                .ForMember(dest => dest.TrialDate, opt => opt.Ignore())
                .ConstructUsing((src, ctx) =>
                {
                    var existingCapable = ctx.Items["ExistingCapable"] as Domain.Entities.Patients.Capable;
                    return new Domain.Entities.Patients.Capable(
                        existingCapable!.CourtDecision,
                        existingCapable!.TrialDate,
                        src.Guardian,
                        src.GuardianOrderAppointment);
                });

            // City //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            CreateMap<byte, City>().ConvertUsing(value => City.FromValue(value));
            CreateMap<City, byte>().ConvertUsing(city => city.Value);

            // DisabilityGroup ///////////////////////////////////////////////////////////////////////////////////////////////////////////
            CreateMap<byte, DisabilityGroup>().ConvertUsing(value => DisabilityGroup.FromValue(value));
            CreateMap<DisabilityGroup, byte>().ConvertUsing(disabilityGroup => disabilityGroup.Value);

            // Pension ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            CreateMap<PensionDTO, Domain.Entities.Patients.Pension>();

            // PensionAddress ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            CreateMap<byte, PensionAddress>().ConvertUsing(value => PensionAddress.FromValue(value));
            CreateMap<PensionAddress, byte>().ConvertUsing(pensionAddress => pensionAddress.Value);

            // Assignment ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            CreateMap<Assignment, EditPatientDTO>();
        }
    }
}