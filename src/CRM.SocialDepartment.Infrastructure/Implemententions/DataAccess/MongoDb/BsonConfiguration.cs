using System;
using CRM.SocialDepartment.Domain.Entities.Patients;
using CRM.SocialDepartment.Domain.Entities.Patients.Documents;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;

namespace CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb
{
    public static class BsonConfiguration
    {
        private static bool _isRegistered = false;

        public static void RegisterMappings()
        {
            if (_isRegistered) return;

            Console.WriteLine("🔧 [BsonConfiguration] Начинаем регистрацию BSON маппингов...");

            // 1. Настройка конвенций для enum
            var conventionPack = new ConventionPack
            {
                new EnumRepresentationConvention(BsonType.String) // Сериализуем enum как строки
            };
            ConventionRegistry.Register("EnumStringConvention", conventionPack, t => true);
            Console.WriteLine("✅ [BsonConfiguration] Enum конвенции зарегистрированы");

            // 2. Настройка сериализатора для DocumentType ValueObject
            if (!BsonClassMap.IsClassMapRegistered(typeof(DocumentType)))
            {
                BsonClassMap.RegisterClassMap<DocumentType>(cm =>
                {
                    cm.AutoMap();
                    cm.MapProperty(c => c.Value);
                    cm.MapProperty(c => c.DisplayName);
                });
                Console.WriteLine("✅ [BsonConfiguration] DocumentType ValueObject зарегистрирован");
            }

            // 3. Настройка ObjectSerializer для полиморфных Document типов
            var objectSerializer = new ObjectSerializer(type => ObjectSerializer.DefaultAllowedTypes(type) || 
                                                               type.IsSubclassOf(typeof(DocumentType)) || 
                                                               type == typeof(DocumentType) ||
                                                               type == typeof(PassportDocument) ||
                                                               type == typeof(MedicalPolicyDocument) ||
                                                               type == typeof(SnilsDocument));
            
            // Проверяем, не зарегистрирован ли уже ObjectSerializer
            try 
            {
                BsonSerializer.RegisterSerializer(typeof(object), objectSerializer);
                Console.WriteLine("✅ [BsonConfiguration] ObjectSerializer настроен для Document типов");
            }
            catch (BsonSerializationException ex) when (ex.Message.Contains("already a serializer registered"))
            {
                Console.WriteLine("⚠️ [BsonConfiguration] ObjectSerializer уже зарегистрирован - используем существующий");
            }

            // 4. Простая конфигурация Document иерархии - только автомаппинг
            if (!BsonClassMap.IsClassMapRegistered(typeof(PassportDocument)))
            {
                BsonClassMap.RegisterClassMap<PassportDocument>(cm => cm.AutoMap());
                Console.WriteLine("✅ [BsonConfiguration] PassportDocument зарегистрирован (простая конфигурация)");
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(MedicalPolicyDocument)))
            {
                BsonClassMap.RegisterClassMap<MedicalPolicyDocument>(cm => cm.AutoMap());
                Console.WriteLine("✅ [BsonConfiguration] MedicalPolicyDocument зарегистрирован (простая конфигурация)");
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(SnilsDocument)))
            {
                BsonClassMap.RegisterClassMap<SnilsDocument>(cm => cm.AutoMap());
                Console.WriteLine("✅ [BsonConfiguration] SnilsDocument зарегистрирован (простая конфигурация)");
            }

            // 5. Настройка Value Objects
            if (!BsonClassMap.IsClassMapRegistered(typeof(HospitalizationType)))
            {
                BsonClassMap.RegisterClassMap<HospitalizationType>(cm =>
                {
                    cm.AutoMap();
                    cm.MapProperty(c => c.Value);
                    cm.MapProperty(c => c.DisplayName);
                });
                Console.WriteLine("✅ [BsonConfiguration] HospitalizationType зарегистрирован");
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(CitizenshipType)))
            {
                BsonClassMap.RegisterClassMap<CitizenshipType>(cm =>
                {
                    cm.AutoMap();
                    cm.MapProperty(c => c.Value);
                    cm.MapProperty(c => c.DisplayName);
                });
                Console.WriteLine("✅ [BsonConfiguration] CitizenshipType зарегистрирован");
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(CityType)))
            {
                BsonClassMap.RegisterClassMap<CityType>(cm =>
                {
                    cm.AutoMap();
                    cm.MapProperty(c => c.Value);
                    cm.MapProperty(c => c.DisplayName);
                });
                Console.WriteLine("✅ [BsonConfiguration] City зарегистрирован");
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(DisabilityGroupType)))
            {
                BsonClassMap.RegisterClassMap<DisabilityGroupType>(cm =>
                {
                    cm.AutoMap();
                    cm.MapProperty(c => c.Value);
                    cm.MapProperty(c => c.DisplayName);
                });
                Console.WriteLine("✅ [BsonConfiguration] DisabilityGroup зарегистрирован");
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(PensionAddressType)))
            {
                BsonClassMap.RegisterClassMap<PensionAddressType>(cm =>
                {
                    cm.AutoMap();
                    cm.MapProperty(c => c.Value);
                    cm.MapProperty(c => c.DisplayName);
                });
                Console.WriteLine("✅ [BsonConfiguration] PensionAddress зарегистрирован");
            }

            // 6. Настройка сложных объектов
            if (!BsonClassMap.IsClassMapRegistered(typeof(MedicalHistory)))
            {
                BsonClassMap.RegisterClassMap<MedicalHistory>(cm =>
                {
                    cm.AutoMap();
                    cm.MapProperty(c => c.NumberDepartment);
                    cm.MapProperty(c => c.HospitalizationType);
                    cm.MapProperty(c => c.Resolution);
                    cm.MapProperty(c => c.NumberDocument);
                    cm.MapProperty(c => c.DateOfReceipt);
                    cm.MapProperty(c => c.DateOfDischarge);
                    cm.MapProperty(c => c.Note);
                });
                Console.WriteLine("✅ [BsonConfiguration] MedicalHistory зарегистрирован");
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(CitizenshipInfo)))
            {
                BsonClassMap.RegisterClassMap<CitizenshipInfo>(cm =>
                {
                    cm.AutoMap();
                    cm.MapProperty(c => c.Citizenship);
                    cm.MapProperty(c => c.Country);
                    cm.MapProperty(c => c.Registration);
                    cm.MapProperty(c => c.EarlyRegistration);
                    cm.MapProperty(c => c.PlaceOfBirth);
                    cm.MapProperty(c => c.DocumentAttached);
                    cm.MapProperty(c => c.NotRegistered);
                });
                Console.WriteLine("✅ [BsonConfiguration] CitizenshipInfo зарегистрирован");
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(Capable)))
            {
                BsonClassMap.RegisterClassMap<Capable>(cm =>
                {
                    cm.AutoMap();
                    cm.MapProperty(c => c.CourtDecision);
                    cm.MapProperty(c => c.TrialDate);
                    cm.MapProperty(c => c.Guardian);
                    cm.MapProperty(c => c.GuardianOrderAppointment);
                });
                Console.WriteLine("✅ [BsonConfiguration] Capable зарегистрирован");
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(Pension)))
            {
                BsonClassMap.RegisterClassMap<Pension>(cm =>
                {
                    cm.AutoMap();
                    cm.MapProperty(c => c.DisabilityGroup);
                    cm.MapProperty(c => c.PensionStartDateTime);
                    cm.MapProperty(c => c.PensionAddress);
                    cm.MapProperty(c => c.SfrBranch);
                    cm.MapProperty(c => c.SfrDepartment);
                    cm.MapProperty(c => c.Rsd);
                });
                Console.WriteLine("✅ [BsonConfiguration] Pension зарегистрирован");
            }

            // 7. Настройка класса Patient с явным маппингом всех свойств
            if (!BsonClassMap.IsClassMapRegistered(typeof(Patient)))
            {
                BsonClassMap.RegisterClassMap<Patient>((Action<BsonClassMap<Patient>>)(cm =>
                {
                    cm.AutoMap();
                    
                    // Явно маппим только свойства из Patient (не из базового класса)
                    cm.MapProperty(c => c.FullName);
                    cm.MapProperty(c => c.Birthday);
                    cm.MapProperty(c => c.CitizenshipInfo);
                    cm.MapProperty(c => c.Capable);
                    cm.MapProperty(c => c.Pension);
                    cm.MapProperty(c => c.Note);
                    cm.MapProperty(c => c.SoftDeleted);
                    cm.MapProperty(c => c.IsArchive);
                    
                    // Маппим MedicalHistories как коллекцию
                    cm.MapProperty(c => c.MedicalHistories);
                    
                    // Простая настройка Documents как ArrayOfArrays без сложных сериализаторов
                    cm.GetMemberMap(c => c.Documents)
                      .SetSerializer(new DictionaryInterfaceImplementerSerializer<Dictionary<DocumentType, object>>(
                          DictionaryRepresentation.ArrayOfArrays));
                }));
                Console.WriteLine("✅ [BsonConfiguration] Patient класс зарегистрирован с полным маппингом");
            }

            _isRegistered = true;
            Console.WriteLine("🎉 [BsonConfiguration] Полная BSON конфигурация завершена!");
        }
    }
}
