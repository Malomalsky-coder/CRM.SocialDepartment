using AutoMapper;
using CRM.SocialDepartment.Application.DTOs;
using CRM.SocialDepartment.Domain.Entities;
using CRM.SocialDepartment.Domain.Entities.Patients;
using CRM.SocialDepartment.Domain.Entities.Patients.Documents;
using CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Data;
using CRM.SocialDepartment.Site.Models.Patient;
using CRM.SocialDepartment.Site.ViewModels.Patient;
using CRM.SocialDepartment.Site.ViewModels.User;
using CRM.SocialDepartment.Site.ViewModels.Role;
using CRM.SocialDepartment.Site.ViewModels.UserActivityLog;

namespace CRM.SocialDepartment.Site.MappingProfile
{
    public class ProjectMappingProfile : Profile
    {
        public ProjectMappingProfile()
        {
            // Patient //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            CreateMap<CreatePatientViewModel, CreatePatientDTO>()
                .ForMember(dest => dest.Birthday, opt => opt.MapFrom(src => 
                    DateTime.SpecifyKind(src.Birthday, DateTimeKind.Utc)))
                .ForMember(dest => dest.Documents, opt => opt.MapFrom(src =>
                    src.Documents.ToDictionary(
                        kvp => kvp.Key,
                        kvp => new DocumentDTO
                        {
                            Type = kvp.Key,
                            // Селективная обработка форматов документов
                            Number = FormatDocumentNumber(kvp.Key, kvp.Value.Number)
                        }
                    )))
                // ВАЖНО: Если пациент НЕ получает пенсию, то данные о пенсии должны быть null
                .ForMember(dest => dest.Pension, opt => opt.MapFrom(src => 
                    src.ReceivesPension ? src.Pension : null))
                // ВАЖНО: Если пациент дееспособен, то Capable должен быть null
                .ForMember(dest => dest.Capable, opt => opt.MapFrom(src => 
                    src.IsCapable ? null : src.Capable))
                // ВАЖНО: Передаем ReceivesPension для корректной обработки
                .ForMember(dest => dest.ReceivesPension, opt => opt.MapFrom(src => src.ReceivesPension));

            CreateMap<EditPatientViewModel, EditPatientDTO>()
                //.ForMember(dest => dest.Birthday, opt => opt.MapFrom(src => 
                //    DateTime.SpecifyKind(src.Birthday, DateTimeKind.Utc)))
                .ForMember(dest => dest.Documents, opt => opt.MapFrom(src =>
                    src.Documents.ToDictionary(
                        kvp => kvp.Key,
                        kvp => new DocumentDTO
                        {
                            Type = kvp.Key,
                            // Селективная обработка форматов документов
                            Number = FormatDocumentNumber(kvp.Key, kvp.Value.Number)
                        }
                    )));

            CreateMap<Patient, EditPatientDTO>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                //.ForMember(dest => dest.Birthday, opt => opt.MapFrom(src => 
                //    DateTime.SpecifyKind(src.Birthday, DateTimeKind.Utc)))
                .ForMember(dest => dest.ActiveMedicalHistory, opt => opt.MapFrom(src => new MedicalHistoryDTO
                {
                    NumberDepartment = (sbyte)(src.ActiveHistory != null ? src.ActiveHistory.NumberDepartment : 0),
                    HospitalizationType = new HospitalizationTypeDTO
                    {
                        Value = (byte)(src.ActiveHistory != null && src.ActiveHistory.HospitalizationType != null ? src.ActiveHistory.HospitalizationType.Value : 0),
                        DisplayName = src.ActiveHistory != null && src.ActiveHistory.HospitalizationType != null ? src.ActiveHistory.HospitalizationType.DisplayName : string.Empty
                    },
                    Resolution = src.ActiveHistory != null ? src.ActiveHistory.Resolution : string.Empty,
                    NumberDocument = src.ActiveHistory != null ? src.ActiveHistory.NumberDocument : string.Empty,
                    DateOfReceipt = src.ActiveHistory != null ? DateTime.SpecifyKind(src.ActiveHistory.DateOfReceipt, DateTimeKind.Utc) : DateTime.MinValue,
                    DateOfDischarge = src.ActiveHistory != null && src.ActiveHistory.DateOfDischarge.HasValue ? DateTime.SpecifyKind(src.ActiveHistory.DateOfDischarge.Value, DateTimeKind.Utc) : null,
                    Note = src.ActiveHistory != null ? src.ActiveHistory.Note : null
                }))
                .ForMember(dest => dest.CitizenshipInfo, opt => opt.MapFrom(src => new CitizenshipInfoDTO
                {
                    Citizenship = src.CitizenshipInfo != null ? src.CitizenshipInfo.Citizenship : CitizenshipType.RussianFederation,
                    Country = src.CitizenshipInfo != null ? src.CitizenshipInfo.Country : string.Empty,
                    Registration = src.CitizenshipInfo != null ? src.CitizenshipInfo.Registration : string.Empty,
                    EarlyRegistration = src.CitizenshipInfo != null ? src.CitizenshipInfo.EarlyRegistration : null,
                    PlaceOfBirth = src.CitizenshipInfo != null ? src.CitizenshipInfo.PlaceOfBirth : null,
                    DocumentAttached = src.CitizenshipInfo != null ? src.CitizenshipInfo.DocumentAttached : null
                }))
                .ForMember(dest => dest.Documents, opt => opt.MapFrom(src => 
                    src.Documents.ToDictionary(
                        kvp => kvp.Key,
                        kvp => new DocumentDTO
                        {
                            Type = kvp.Key,
                            Number = ExtractDocumentNumber(kvp.Value)
                        }
                    )))
                .ForMember(dest => dest.Capable, opt => opt.MapFrom(src => src.Capable != null ? new EditCapableDTO
                {
                    Guardian = src.Capable.Guardian != null ? src.Capable.Guardian : string.Empty,
                    GuardianOrderAppointment = src.Capable.GuardianOrderAppointment != null ? src.Capable.GuardianOrderAppointment : string.Empty
                } : null))
                .ForMember(dest => dest.Pension, opt => opt.MapFrom(src => src.Pension != null ? new PensionDTO
                {
                    DisabilityGroup = src.Pension.DisabilityGroup,
                    PensionStartDateTime = src.Pension.PensionStartDateTime.HasValue ? DateTime.SpecifyKind(src.Pension.PensionStartDateTime.Value, DateTimeKind.Utc) : null,
                    PensionAddress = src.Pension.PensionAddress,
                    SfrBranch = src.Pension.SfrBranch,
                    SfrDepartment = src.Pension.SfrDepartment,
                    Rsd = src.Pension.Rsd
                } : null))
                .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note));

            CreateMap<Patient, CreatePatientDTO>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Birthday, opt => opt.MapFrom(src => 
                    DateTime.SpecifyKind(src.Birthday, DateTimeKind.Utc)))
                .ForMember(dest => dest.MedicalHistory, opt => opt.MapFrom(src => new MedicalHistoryDTO
                {
                    NumberDepartment = (sbyte)(src.ActiveHistory != null ? src.ActiveHistory.NumberDepartment : 0),
                    HospitalizationType = new HospitalizationTypeDTO
                    {
                        Value = (byte)(src.ActiveHistory != null && src.ActiveHistory.HospitalizationType != null ? src.ActiveHistory.HospitalizationType.Value : 0),
                        DisplayName = src.ActiveHistory != null && src.ActiveHistory.HospitalizationType != null ? src.ActiveHistory.HospitalizationType.DisplayName : string.Empty
                    },
                    Resolution = src.ActiveHistory != null ? src.ActiveHistory.Resolution : string.Empty,
                    NumberDocument = src.ActiveHistory != null ? src.ActiveHistory.NumberDocument : string.Empty,
                    DateOfReceipt = src.ActiveHistory != null ? DateTime.SpecifyKind(src.ActiveHistory.DateOfReceipt, DateTimeKind.Utc) : DateTime.MinValue,
                    DateOfDischarge = src.ActiveHistory != null && src.ActiveHistory.DateOfDischarge.HasValue ? DateTime.SpecifyKind(src.ActiveHistory.DateOfDischarge.Value, DateTimeKind.Utc) : null,
                    Note = src.ActiveHistory != null ? src.ActiveHistory.Note : null
                }))
                .ForMember(dest => dest.CitizenshipInfo, opt => opt.MapFrom(src => new CitizenshipInfoDTO
                {
                    Citizenship = src.CitizenshipInfo != null ? src.CitizenshipInfo.Citizenship : CitizenshipType.RussianFederation,
                    Country = src.CitizenshipInfo != null ? src.CitizenshipInfo.Country : string.Empty,
                    Registration = src.CitizenshipInfo != null ? src.CitizenshipInfo.Registration : string.Empty,
                    EarlyRegistration = src.CitizenshipInfo != null ? src.CitizenshipInfo.EarlyRegistration : null,
                    PlaceOfBirth = src.CitizenshipInfo != null ? src.CitizenshipInfo.PlaceOfBirth : null,
                    DocumentAttached = src.CitizenshipInfo != null ? src.CitizenshipInfo.DocumentAttached : null
                }))
                .ForMember(dest => dest.Documents, opt => opt.MapFrom(src => 
                    src.Documents.ToDictionary(
                        kvp => kvp.Key,
                        kvp => new DocumentDTO
                        {
                            Type = kvp.Key,
                            Number = ExtractDocumentNumber(kvp.Value)
                        }
                    )))
                .ForMember(dest => dest.IsCapable, opt => opt.MapFrom(src => src.IsCapable))
                .ForMember(dest => dest.Capable, opt => opt.MapFrom(src => src.Capable != null ? new CreateCapableDTO
                {
                    CourtDecision = src.Capable.CourtDecision != null ? src.Capable.CourtDecision : string.Empty,
                    TrialDate = src.Capable.TrialDate.HasValue ? DateTime.SpecifyKind(src.Capable.TrialDate.Value, DateTimeKind.Utc) : null,
                    Guardian = src.Capable.Guardian != null ? src.Capable.Guardian : string.Empty,
                    GuardianOrderAppointment = src.Capable.GuardianOrderAppointment != null ? src.Capable.GuardianOrderAppointment : string.Empty
                } : null))
                .ForMember(dest => dest.ReceivesPension, opt => opt.MapFrom(src => src.ReceivesPension))
                .ForMember(dest => dest.Pension, opt => opt.MapFrom(src => src.Pension != null ? new PensionDTO
                {
                    DisabilityGroup = src.Pension.DisabilityGroup,
                    PensionStartDateTime = src.Pension.PensionStartDateTime.HasValue ? DateTime.SpecifyKind(src.Pension.PensionStartDateTime.Value, DateTimeKind.Utc) : null,
                    PensionAddress = src.Pension.PensionAddress,
                    SfrBranch = src.Pension.SfrBranch,
                    SfrDepartment = src.Pension.SfrDepartment,
                    Rsd = src.Pension.Rsd
                } : null))
                .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note));

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

            CreateMap<EditPatientViewModel, EditPatientDTO>();
            CreateMap<CreateUserViewModel, CreateUserDTO>();
            CreateMap<EditUserViewModel, CreateUserDTO>();
            CreateMap<IUser, UserDTO>();
            CreateMap<ApplicationUser, UserDTO>();
            
            // Маппинг для пользователей
            CreateMap<ApplicationUser, EditUserViewModel>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
                .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position))
                .ForMember(dest => dest.DepartmentNumber, opt => opt.MapFrom(src => src.DepartmentNumber))
                .ForMember(dest => dest.Password, opt => opt.Ignore()) // Пароль не маппим из базы
                .ForMember(dest => dest.ConfirmPassword, opt => opt.Ignore()); // Подтверждение пароля не маппим из базы
                
            CreateMap<UserDTO, EditUserViewModel>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
                .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position))
                .ForMember(dest => dest.DepartmentNumber, opt => opt.MapFrom(src => src.DepartmentNumber))
                .ForMember(dest => dest.Password, opt => opt.Ignore()) // Пароль не маппим из DTO
                .ForMember(dest => dest.ConfirmPassword, opt => opt.Ignore()); // Подтверждение пароля не маппим из DTO

            // Маппинг для ролей
            CreateMap<CreateRoleViewModel, CreateRoleDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
            CreateMap<EditRoleViewModel, CreateRoleDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
            CreateMap<IRole, RoleDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Empty))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.NormalizedName, opt => opt.MapFrom(src => src.Name.ToUpperInvariant()))
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<ApplicationRole, RoleDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.NormalizedName, opt => opt.MapFrom(src => src.NormalizedName))
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn));
            CreateMap<RoleDTO, EditRoleViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

            // Маппинг DTO в ViewModels с правильной обработкой дат
            CreateMap<CreatePatientDTO, CreatePatientViewModel>()
                .ForMember(dest => dest.Birthday, opt => opt.MapFrom(src => 
                    DateTime.SpecifyKind(src.Birthday, DateTimeKind.Utc)));

            CreateMap<EditPatientDTO, EditPatientViewModel>();
                //.ForMember(dest => dest.Birthday, opt => opt.MapFrom(src => 
                //    DateTime.SpecifyKind(src.Birthday, DateTimeKind.Utc)));

            // ❗ КРИТИЧЕСКИ ВАЖНЫЕ МАППИНГИ ДЛЯ ViewModel -> DTO ❗ //
            
            // MedicalHistory (ViewModel) -> MedicalHistoryDTO
            CreateMap<MedicalHistoryModel, MedicalHistoryDTO>()
                .ForMember(dest => dest.DateOfReceipt, opt => opt.MapFrom(src => 
                    DateTime.SpecifyKind(src.DateOfReceipt, DateTimeKind.Utc)))
                .ForMember(dest => dest.DateOfDischarge, opt => opt.MapFrom(src => 
                    src.DateOfDischarge.HasValue ? DateTime.SpecifyKind(src.DateOfDischarge.Value, DateTimeKind.Utc) : (DateTime?)null))
                .ForMember(dest => dest.HospitalizationType, opt => opt.MapFrom(src => 
                    src.HospitalizationType != null 
                        ? new HospitalizationTypeDTO
                        {
                            Value = src.HospitalizationType.Value,
                            DisplayName = src.HospitalizationType.DisplayName
                        }
                        : new HospitalizationTypeDTO  // Значение по умолчанию
                        {
                            Value = 1,  // Force
                            DisplayName = "Принудительно"
                        }));

            // CitizenshipInfo (ViewModel) -> CitizenshipInfoDTO
            CreateMap<CitizenshipInfoModel, CitizenshipInfoDTO>()
                .ForMember(dest => dest.Citizenship, opt => opt.MapFrom(src => 
                    src.Citizenship != null ? src.Citizenship : CitizenshipType.RussianFederation)) // Значение по умолчанию
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
                .ForMember(dest => dest.Registration, opt => opt.MapFrom(src => src.Registration))
                .ForMember(dest => dest.EarlyRegistration, opt => opt.MapFrom(src => src.EarlyRegistration))
                .ForMember(dest => dest.PlaceOfBirth, opt => opt.MapFrom(src => src.PlaceOfBirth))
                .ForMember(dest => dest.DocumentAttached, opt => opt.MapFrom(src => src.DocumentAttached));

            // Capable (ViewModel) -> CreateCapableDTO/EditCapableDTO
            CreateMap<CapableModel, CreateCapableDTO>()
                .ForMember(dest => dest.TrialDate, opt => opt.MapFrom(src => 
                    src.TrialDate.HasValue ? DateTime.SpecifyKind(src.TrialDate.Value, DateTimeKind.Utc) : (DateTime?)null));
            CreateMap<CapableModel, EditCapableDTO>()
                .ForMember(dest => dest.TrialDate, opt => opt.MapFrom(src => 
                    src.TrialDate.HasValue ? DateTime.SpecifyKind(src.TrialDate.Value, DateTimeKind.Utc) : (DateTime?)null));

            // Pension (ViewModel) -> PensionDTO
            CreateMap<PensionModel, PensionDTO>()
                .ForMember(dest => dest.PensionStartDateTime, opt => opt.MapFrom(src => 
                    src.PensionStartDateTime.HasValue ? DateTime.SpecifyKind(src.PensionStartDateTime.Value, DateTimeKind.Utc) : (DateTime?)null));

            // HospitalizationType //////////////////////////////////////////////////////////////////////////////////////////////////////
            CreateMap<byte, HospitalizationType>().ConvertUsing(value => HospitalizationType.FromValue(value));
            CreateMap<HospitalizationType, byte>().ConvertUsing(hospitalizationType => hospitalizationType.Value);

            // CitizenshipInfo ///////////////////////////////////////////////////////////////////////////////////////////////////////////
            CreateMap<CitizenshipInfoDTO, CitizenshipInfo>().ReverseMap();

            // CitizenshipType ///////////////////////////////////////////////////////////////////////////////////////////////////////////
            CreateMap<byte, CitizenshipType>().ConvertUsing(value => CitizenshipType.FromValue(value));
            CreateMap<CitizenshipType, byte>().ConvertUsing(citizenshipType => citizenshipType.Value);

            // Capable //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            CreateMap<CreateCapableDTO, Capable>()
                .ConstructUsing(src => new Capable(
                    src.CourtDecision,
                    src.TrialDate.HasValue ? DateTime.SpecifyKind(src.TrialDate.Value, DateTimeKind.Utc) : null,
                    src.Guardian,
                    src.GuardianOrderAppointment));

            CreateMap<EditCapableDTO, Capable>()
                .ForMember(dest => dest.CourtDecision, opt => opt.Ignore())
                .ForMember(dest => dest.TrialDate, opt => opt.Ignore())
                .ConstructUsing((src, ctx) =>
                {
                    var existingCapable = ctx.Items["ExistingCapable"] as Capable;
                    return new Capable(
                        existingCapable!.CourtDecision,
                        existingCapable!.TrialDate.HasValue ? DateTime.SpecifyKind(existingCapable.TrialDate.Value, DateTimeKind.Utc) : null,
                        src.Guardian,
                        src.GuardianOrderAppointment);
                });

            // City //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            CreateMap<byte, CityType>().ConvertUsing(value => CityType.FromValue(value));
            CreateMap<CityType, byte>().ConvertUsing(city => city.Value);

            // DisabilityGroup ///////////////////////////////////////////////////////////////////////////////////////////////////////////
            CreateMap<byte, DisabilityGroupType>().ConvertUsing(value => DisabilityGroupType.FromValue(value));
            CreateMap<DisabilityGroupType, byte>().ConvertUsing(disabilityGroup => disabilityGroup.Value);

            // Pension ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            CreateMap<PensionDTO, Pension>()
                .ForMember(dest => dest.PensionStartDateTime, opt => opt.MapFrom(src => 
                    src.PensionStartDateTime.HasValue ? DateTime.SpecifyKind(src.PensionStartDateTime.Value, DateTimeKind.Utc) : (DateTime?)null));

            // PensionAddress ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            CreateMap<byte, PensionAddressType>().ConvertUsing(value => PensionAddressType.FromValue(value));
            CreateMap<PensionAddressType, byte>().ConvertUsing(pensionAddress => pensionAddress.Value);

            // Assignment ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            CreateMap<Assignment, EditPatientDTO>();

            CreateMap<Patient, ActivePatientDTO>()
                .ForMember(dest => dest.HospitalizationType, opt => opt.MapFrom(src =>
                    src.ActiveHistory != null && src.ActiveHistory.HospitalizationType != null
                        ? src.ActiveHistory.HospitalizationType.DisplayName
                        : "—"))
                .ForMember(dest => dest.CourtDecision, opt => opt.MapFrom(src =>
                    src.Capable != null && !string.IsNullOrEmpty(src.Capable.CourtDecision)
                        ? src.Capable.CourtDecision
                        : "—"))
                .ForMember(dest => dest.NumberDocument, opt => opt.MapFrom(src =>
                    src.ActiveHistory != null && !string.IsNullOrEmpty(src.ActiveHistory.NumberDocument)
                        ? src.ActiveHistory.NumberDocument
                        : "—"))
                .ForMember(dest => dest.DateOfReceipt, opt => opt.MapFrom(src =>
                    src.ActiveHistory != null ? DateTime.SpecifyKind(src.ActiveHistory.DateOfReceipt, DateTimeKind.Utc) : DateTime.MinValue))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src =>
                    src.ActiveHistory != null && src.ActiveHistory.NumberDepartment.HasValue
                        ? src.ActiveHistory.NumberDepartment.Value.ToString()
                        : "—"))
                .ForMember(dest => dest.IsChildren, opt => opt.MapFrom(src => src.IsChildren))
                .ForMember(dest => dest.Citizenship, opt => opt.MapFrom(src =>
                    src.CitizenshipInfo != null && src.CitizenshipInfo.Citizenship != null
                        ? src.CitizenshipInfo.Citizenship.DisplayName
                        : "—"))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src =>
                    src.CitizenshipInfo != null && !string.IsNullOrEmpty(src.CitizenshipInfo.Country)
                        ? src.CitizenshipInfo.Country
                        : "—"))
                .ForMember(dest => dest.Registration, opt => opt.MapFrom(src =>
                    src.CitizenshipInfo != null && !string.IsNullOrEmpty(src.CitizenshipInfo.Registration)
                        ? src.CitizenshipInfo.Registration
                        : "—"))
                .ForMember(dest => dest.IsHomeless, opt => opt.MapFrom(src =>
                    src.CitizenshipInfo != null && src.CitizenshipInfo.NotRegistered
                        ? true
                        : false))
                .ForMember(dest => dest.EarlyRegistration, opt => opt.MapFrom(src =>
                    src.CitizenshipInfo != null && src.CitizenshipInfo.EarlyRegistration != null
                        ? src.CitizenshipInfo.EarlyRegistration.DisplayName
                        : "—"))
                .ForMember(dest => dest.PlaceOfBirth, opt => opt.MapFrom(src =>
                    src.CitizenshipInfo != null && !string.IsNullOrEmpty(src.CitizenshipInfo.PlaceOfBirth)
                        ? src.CitizenshipInfo.PlaceOfBirth
                        : "—"))
                .ForMember(dest => dest.IsCapable, opt => opt.MapFrom(src => src.IsCapable))
                .ForMember(dest => dest.ReceivesPension, opt => opt.MapFrom(src => src.ReceivesPension))
                .ForMember(dest => dest.DisabilityGroup, opt => opt.MapFrom(src =>
                    src.Pension != null && src.Pension.DisabilityGroup != null && src.Pension.DisabilityGroup.Value > 0
                        ? src.Pension.DisabilityGroup.Value.ToString()
                        : "—"))
                .ForMember(dest => dest.Note, opt => opt.MapFrom(src =>
                    !string.IsNullOrEmpty(src.Note)
                        ? src.Note
                        : "—"));

            // UserActivityLog ///////////////////////////////////////////////////////////////////////////////////////////////////////////
            CreateMap<UserActivityLog, UserActivityLogDTO>()
                .ForMember(dest => dest.ActivityTypeName, opt => opt.MapFrom(src => 
                    GetActivityTypeName(src.ActivityType)))
                .ForMember(dest => dest.FormattedTimestamp, opt => opt.MapFrom(src => 
                    src.Timestamp.ToString("dd.MM.yyyy HH:mm:ss")));

            CreateMap<UserActivityLogFilterViewModel, UserActivityLogFilterDTO>().ReverseMap();
        }

        /// <summary>
        /// Получает название типа активности
        /// </summary>
        /// <param name="activityType">Тип активности</param>
        /// <returns>Название типа активности</returns>
        private static string GetActivityTypeName(UserActivityType activityType)
        {
            return activityType switch
            {
                UserActivityType.Login => "Авторизация",
                UserActivityType.Logout => "Выход",
                UserActivityType.DataRequest => "Запрос данных",
                UserActivityType.Create => "Создание",
                UserActivityType.Update => "Редактирование",
                UserActivityType.Delete => "Удаление",
                _ => activityType.ToString()
            };
        }

        /// <summary>
        /// Извлекает номер документа из object в Dictionary
        /// </summary>
        /// <param name="document">Документ как object</param>
        /// <returns>Номер документа или пустая строка</returns>
        private static string ExtractDocumentNumber(object document)
        {
            if (document == null) return string.Empty;
            
            if (document is PassportDocument passport)
                return passport.Number != null ? passport.Number : string.Empty;
            if (document is MedicalPolicyDocument policy)
                return policy.Number != null ? policy.Number : string.Empty;
            if (document is SnilsDocument snils)
                return snils.Number != null ? snils.Number : string.Empty;
            
            return string.Empty;
        }

        /// <summary>
        /// Форматирует номер документа в зависимости от его типа
        /// </summary>
        /// <param name="documentType">Тип документа</param>
        /// <param name="number">Номер документа от пользователя</param>
        /// <returns>Отформатированный номер для хранения в базе</returns>
        private static string FormatDocumentNumber(DocumentType documentType, string? number)
        {
            if (string.IsNullOrEmpty(number))
                return string.Empty;

            return documentType switch
            {
                var dt when dt == DocumentType.Passport => number,
                var dt when dt == DocumentType.MedicalPolicy => number.Replace(" ", ""),
                var dt when dt == DocumentType.Snils => number,
                _ => number
            };
        }
    }
}