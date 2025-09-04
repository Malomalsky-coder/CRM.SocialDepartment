using CRM.SocialDepartment.Domain.Entities;
using CRM.SocialDepartment.Domain.Entities.Patients;
using CRM.SocialDepartment.Domain.Entities.Patients.Documents;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using System.Reflection;

namespace CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb
{
    /// <summary>
    /// Кастомный сериализатор для Patient класса
    /// </summary>
    public class PatientSerializer : SerializerBase<Patient>, IBsonDocumentSerializer
    {
        public override Patient Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            try
            {
                var bsonType = context.Reader.GetCurrentBsonType();
                
                if (bsonType == BsonType.Null)
                {
                    context.Reader.ReadNull();
                    return null;
                }
                
                if (bsonType == BsonType.Document)
                {
                    var document = BsonDocumentSerializer.Instance.Deserialize(context, args);
                    Console.WriteLine($"🔍 [PatientSerializer] Десериализация Patient из документа");
                
                // Создаем Patient через рефлексию, чтобы обойти приватные конструкторы
                var patient = (Patient)Activator.CreateInstance(typeof(Patient), true);
                
                if (patient == null)
                {
                    Console.WriteLine("❌ [PatientSerializer] Не удалось создать экземпляр Patient");
                    return null;
                }
                
                Console.WriteLine($"✅ [PatientSerializer] Создан экземпляр Patient: {patient.GetType().Name}");
                
                // Устанавливаем базовые свойства
                if (document.Contains("_id"))
                {
                    var idValue = document["_id"].AsGuid;
                    typeof(Patient).GetField("_id", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(patient, idValue);
                    typeof(Patient).GetProperty("Id")?.SetValue(patient, idValue);
                    Console.WriteLine($"✅ [PatientSerializer] Установлен _id: {idValue}");
                }
                
                if (document.Contains("FullName"))
                {
                    typeof(Patient).GetProperty("FullName")?.SetValue(patient, document["FullName"].AsString);
                    Console.WriteLine($"✅ [PatientSerializer] Установлен FullName: {document["FullName"].AsString}");
                }
                
                if (document.Contains("Birthday"))
                {
                    typeof(Patient).GetProperty("Birthday")?.SetValue(patient, document["Birthday"].ToUniversalTime());
                    Console.WriteLine($"✅ [PatientSerializer] Установлен Birthday: {document["Birthday"].ToUniversalTime()}");
                }
                
                // Десериализуем MedicalHistories
                var histories = new List<MedicalHistory>();
                
                // Сначала проверяем ActiveHistory (объект)
                if (document.Contains("ActiveHistory") && document["ActiveHistory"].IsBsonDocument)
                {
                    var activeHistory = DeserializeMedicalHistory(document["ActiveHistory"].AsBsonDocument);
                    if (activeHistory != null)
                    {
                        histories.Add(activeHistory);
                        Console.WriteLine($"✅ [PatientSerializer] Добавлена ActiveHistory: {activeHistory.NumberDepartment}");
                    }
                }
                
                // Затем проверяем MedicalHistories (массив)
                if (document.Contains("MedicalHistories") && document["MedicalHistories"].IsBsonArray)
                {
                    var historiesArray = document["MedicalHistories"].AsBsonArray;
                    Console.WriteLine($"🔍 [PatientSerializer] Найдено {historiesArray.Count} MedicalHistories");
                    
                    foreach (var historyDoc in historiesArray)
                    {
                        if (historyDoc.IsBsonDocument)
                        {
                            var history = DeserializeMedicalHistory(historyDoc.AsBsonDocument);
                            if (history != null)
                            {
                                // Проверяем, не дублируется ли уже ActiveHistory
                                if (!histories.Any(h => h.Id == history.Id))
                                {
                                    histories.Add(history);
                                    Console.WriteLine($"✅ [PatientSerializer] Добавлена MedicalHistory: {history.NumberDepartment}");
                                }
                            }
                        }
                    }
                }
                
                // Устанавливаем через рефлексию
                var medicalHistoriesField = typeof(Patient).GetField("_medicalHistories", BindingFlags.NonPublic | BindingFlags.Instance);
                medicalHistoriesField?.SetValue(patient, histories);
                Console.WriteLine($"✅ [PatientSerializer] Установлено {histories.Count} MedicalHistories");
                
                // Десериализуем CitizenshipInfo
                if (document.Contains("CitizenshipInfo") && document["CitizenshipInfo"].IsBsonDocument)
                {
                    var citizenshipInfo = DeserializeCitizenshipInfo(document["CitizenshipInfo"].AsBsonDocument);
                    if (citizenshipInfo != null)
                    {
                        typeof(Patient).GetProperty("CitizenshipInfo")?.SetValue(patient, citizenshipInfo);
                        Console.WriteLine($"✅ [PatientSerializer] Установлен CitizenshipInfo: {citizenshipInfo.Citizenship?.DisplayName}");
                    }
                    else
                    {
                        Console.WriteLine($"❌ [PatientSerializer] Не удалось десериализовать CitizenshipInfo");
                    }
                }
                else
                {
                    Console.WriteLine($"⚠️ [PatientSerializer] CitizenshipInfo не найден или не является документом");
                }
                
                // Десериализуем Documents
                var documents = new Dictionary<DocumentType, object>();
                
                // Проверяем Documents как объект (новый формат)
                if (document.Contains("Documents") && document["Documents"].IsBsonDocument)
                {
                    var documentsDoc = document["Documents"].AsBsonDocument;
                    
                    foreach (var docElement in documentsDoc)
                    {
                        try
                        {
                            // Парсим DocumentType из имени поля
                            DocumentType? documentType = docElement.Name switch
                            {
                                "Passport" => DocumentType.Passport,
                                "MedicalPolicy" => DocumentType.MedicalPolicy,
                                "Snils" => DocumentType.Snils,
                                _ => null
                            };
                            
                            if (documentType != null)
                            {
                                // Десериализуем значение документа
                                var documentValue = DeserializeDocumentValue(docElement.Value.AsBsonDocument);
                                
                                if (documentValue != null)
                                {
                                    documents[documentType] = documentValue;
                                    Console.WriteLine($"✅ [PatientSerializer] Добавлен документ: {documentType.DisplayName}");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"⚠️ [PatientSerializer] Неизвестный тип документа: {docElement.Name}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"❌ [PatientSerializer] Ошибка десериализации документа {docElement.Name}: {ex.Message}");
                        }
                    }
                }
                // Проверяем Documents как массив (старый формат)
                else if (document.Contains("Documents") && document["Documents"].IsBsonArray)
                {
                    var documentsArray = document["Documents"].AsBsonArray;
                    Console.WriteLine($"🔍 [PatientSerializer] Найдено {documentsArray.Count} Documents (массив)");
                    
                    foreach (var docItem in documentsArray)
                    {
                        if (docItem.IsBsonArray && docItem.AsBsonArray.Count == 2)
                        {
                            var docArray = docItem.AsBsonArray;
                            var documentType = DeserializeDocumentType(docArray[0].AsBsonDocument);
                            var documentValue = DeserializeDocumentValue(docArray[1].AsBsonDocument);
                            
                            if (documentType != null && documentValue != null)
                            {
                                documents[documentType] = documentValue;
                                Console.WriteLine($"✅ [PatientSerializer] Добавлен документ: {documentType.DisplayName}");
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"⚠️ [PatientSerializer] Documents не найдены или имеют неожиданный формат");
                }
                
                // Устанавливаем Documents через рефлексию
                typeof(Patient).GetProperty("Documents")?.SetValue(patient, documents);
                Console.WriteLine($"✅ [PatientSerializer] Установлено {documents.Count} Documents");
                
                // Устанавливаем остальные свойства
                if (document.Contains("SoftDeleted"))
                    typeof(Patient).GetProperty("SoftDeleted")?.SetValue(patient, document["SoftDeleted"].AsBoolean);
                
                if (document.Contains("IsArchive"))
                    typeof(Patient).GetProperty("IsArchive")?.SetValue(patient, document["IsArchive"].AsBoolean);
                
                if (document.Contains("Note") && !document["Note"].IsBsonNull)
                    typeof(Patient).GetProperty("Note")?.SetValue(patient, document["Note"].AsString);
                
                Console.WriteLine($"✅ [PatientSerializer] Десериализация Patient завершена успешно");
                return patient;
                }
                
                throw new FormatException($"Cannot deserialize Patient from BsonType: {bsonType}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [PatientSerializer] Ошибка десериализации Patient: {ex.Message}");
                Console.WriteLine($"❌ [PatientSerializer] Stack trace: {ex.StackTrace}");
                
                // Возвращаем null вместо исключения, чтобы не сломать приложение
                return null;
            }
        }
        
        private MedicalHistory DeserializeMedicalHistory(BsonDocument doc)
        {
            try
            {
                var id = doc.Contains("_id") ? doc["_id"].AsGuid : Guid.NewGuid();
                var numberDepartment = doc.Contains("NumberDepartment") ? doc["NumberDepartment"].AsInt32 : 0;
                var resolution = doc.Contains("Resolution") && !doc["Resolution"].IsBsonNull ? doc["Resolution"].AsString : "";
                var numberDocument = doc.Contains("NumberDocument") && !doc["NumberDocument"].IsBsonNull ? doc["NumberDocument"].AsString : "";
                var dateOfReceipt = doc.Contains("DateOfReceipt") ? doc["DateOfReceipt"].ToUniversalTime() : DateTime.Now;
                var note = doc.Contains("Note") && !doc["Note"].IsBsonNull ? doc["Note"].AsString : null;
                
                // Десериализуем HospitalizationType
                HospitalizationType hospitalizationType = HospitalizationType.Force;
                if (doc.Contains("HospitalizationType") && doc["HospitalizationType"].IsBsonDocument)
                {
                    var typeDoc = doc["HospitalizationType"].AsBsonDocument;
                    var value = typeDoc.Contains("Value") ? typeDoc["Value"].AsInt32 : 0;
                    hospitalizationType = HospitalizationType.FromValue((byte)value);
                }
                
                return new MedicalHistory(id, (sbyte)numberDepartment, hospitalizationType, resolution, numberDocument, dateOfReceipt, note);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [PatientSerializer] Ошибка десериализации MedicalHistory: {ex.Message}");
                return null;
            }
        }
        
        private CitizenshipInfo DeserializeCitizenshipInfo(BsonDocument doc)
        {
            try
            {
                var country = doc.Contains("Country") && !doc["Country"].IsBsonNull ? doc["Country"].AsString : null;
                var registration = doc.Contains("Registration") && !doc["Registration"].IsBsonNull ? doc["Registration"].AsString : null;
                var placeOfBirth = doc.Contains("PlaceOfBirth") && !doc["PlaceOfBirth"].IsBsonNull ? doc["PlaceOfBirth"].AsString : null;
                var documentAttached = doc.Contains("DocumentAttached") && !doc["DocumentAttached"].IsBsonNull ? doc["DocumentAttached"].AsString : null;
                
                // Десериализуем Citizenship
                CitizenshipType citizenship = CitizenshipType.RussianFederation;
                if (doc.Contains("Citizenship") && doc["Citizenship"].IsBsonDocument)
                {
                    var citizenshipDoc = doc["Citizenship"].AsBsonDocument;
                    var value = citizenshipDoc.Contains("Value") ? citizenshipDoc["Value"].AsInt32 : 0;
                    citizenship = CitizenshipType.FromValue((byte)value);
                }
                
                // Десериализуем EarlyRegistration
                CityType? earlyRegistration = null;
                if (doc.Contains("EarlyRegistration") && doc["EarlyRegistration"].IsBsonDocument)
                {
                    var earlyRegDoc = doc["EarlyRegistration"].AsBsonDocument;
                    var value = earlyRegDoc.Contains("Value") ? earlyRegDoc["Value"].AsInt32 : 0;
                    earlyRegistration = CityType.FromValue((byte)value);
                }
                
                return new CitizenshipInfo(citizenship, country, registration, earlyRegistration, placeOfBirth, documentAttached);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [PatientSerializer] Ошибка десериализации CitizenshipInfo: {ex.Message}");
                return null;
            }
        }
        
        private DocumentType? DeserializeDocumentType(BsonDocument doc)
        {
            try
            {
                if (doc.Contains("Value"))
                {
                    var value = doc["Value"].AsInt32;
                    return DocumentType.FromValue((byte)value);
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [PatientSerializer] Ошибка десериализации DocumentType: {ex.Message}");
                return null;
            }
        }
        
        private object? DeserializeDocumentValue(BsonDocument doc)
        {
            try
            {
                // Определяем тип документа по содержимому
                if (doc.Contains("Number"))
                {
                    var number = doc["Number"].AsString;
                    
                    // Определяем конкретный тип документа по дополнительным полям или контексту
                    if (doc.Contains("_t"))
                    {
                        var typeName = doc["_t"].AsString;
                        return typeName switch
                        {
                            "PassportDocument" => new PassportDocument(number),
                            "MedicalPolicyDocument" => new MedicalPolicyDocument(number),
                            "SnilsDocument" => new SnilsDocument(number),
                            _ => new PassportDocument(number) // По умолчанию
                        };
                    }
                    
                    // Если нет информации о типе, создаем PassportDocument по умолчанию
                    return new PassportDocument(number);
                }
                
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [PatientSerializer] Ошибка десериализации DocumentValue: {ex.Message}");
                return null;
            }
        }
        
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Patient value)
        {
            if (value == null)
            {
                context.Writer.WriteNull();
                return;
            }
            
            // Создаем документ и сериализуем его напрямую
            var document = new BsonDocument();
            
            // Сериализуем основные поля
            if (value.Id != null)
                document["_id"] = value.Id;
            if (value.FullName != null)
                document["FullName"] = value.FullName;
            if (value.Birthday != default)
                document["Birthday"] = value.Birthday;
            if (value.ActiveHistory != null)
                document["ActiveHistory"] = BsonDocument.Parse(value.ActiveHistory.ToJson());
            if (value.CitizenshipInfo != null)
                document["CitizenshipInfo"] = BsonDocument.Parse(value.CitizenshipInfo.ToJson());
            // Сериализуем документы (всегда, даже если пустые)
            if (value.Documents != null)
            {
                var documentsDoc = new BsonDocument();
                Console.WriteLine($"🔍 [PatientSerializer] Сериализация {value.Documents.Count} документов");
                
                foreach (var doc in value.Documents)
                {
                    try
                    {
                        var docJson = doc.Value.ToJson();
                        documentsDoc[doc.Key.ToString()] = BsonDocument.Parse(docJson);
                        Console.WriteLine($"✅ [PatientSerializer] Сериализован документ {doc.Key}: {docJson}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ [PatientSerializer] Ошибка сериализации документа {doc.Key}: {ex.Message}");
                    }
                }
                document["Documents"] = documentsDoc;
            }
            else
            {
                Console.WriteLine($"⚠️ [PatientSerializer] Documents is null");
            }
            if (value.Capable != null)
                document["Capable"] = BsonDocument.Parse(value.Capable.ToJson());
            if (value.Pension != null)
                document["Pension"] = BsonDocument.Parse(value.Pension.ToJson());
            if (value.Note != null)
                document["Note"] = value.Note;
            if (value.IsCapable)
                document["IsCapable"] = value.IsCapable;
            if (value.ReceivesPension)
                document["ReceivesPension"] = value.ReceivesPension;

            document["IsArchive"] = value.IsArchive;
            document["SoftDeleted"] = value.SoftDeleted;
            
            // Записываем документ
            context.Writer.WriteStartDocument();
            foreach (var element in document)
            {
                context.Writer.WriteName(element.Name);
                BsonValueSerializer.Instance.Serialize(context, element.Value);
            }
            context.Writer.WriteEndDocument();
        }
        
        public bool TryGetMemberSerializationInfo(string memberName, out BsonSerializationInfo serializationInfo)
        {
            // Возвращаем информацию о сериализации для полей, которые могут использоваться в LINQ запросах
            switch (memberName)
            {
                case "Id":
                    serializationInfo = new BsonSerializationInfo("_id", BsonSerializer.LookupSerializer<Guid>(), typeof(Guid));
                    return true;
                case "FullName":
                    serializationInfo = new BsonSerializationInfo("FullName", BsonSerializer.LookupSerializer<string>(), typeof(string));
                    return true;
                case "SoftDeleted":
                    serializationInfo = new BsonSerializationInfo("SoftDeleted", BsonSerializer.LookupSerializer<bool>(), typeof(bool));
                    return true;
                case "IsArchive":
                    serializationInfo = new BsonSerializationInfo("IsArchive", BsonSerializer.LookupSerializer<bool>(), typeof(bool));
                    return true;
                case "Birthday":
                    serializationInfo = new BsonSerializationInfo("Birthday", BsonSerializer.LookupSerializer<DateTime>(), typeof(DateTime));
                    return true;
                default:
                    serializationInfo = null;
                    return false;
            }
        }
    }

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
                    cm.SetIgnoreExtraElements(true);
                    
                    // Явно маппим свойства с правильными именами
                    cm.MapProperty(c => c.Value).SetElementName("Value");
                    cm.MapProperty(c => c.DisplayName).SetElementName("DisplayName");
                });
                Console.WriteLine("✅ [BsonConfiguration] CitizenshipType зарегистрирован с явным маппингом");
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

            // 7. Настройка класса Assignment с исключением _domainEvents
            if (!BsonClassMap.IsClassMapRegistered(typeof(CRM.SocialDepartment.Domain.Entities.Assignment)))
            {
                BsonClassMap.RegisterClassMap<CRM.SocialDepartment.Domain.Entities.Assignment>(cm =>
                {
                    cm.AutoMap();
                    
                    // Исключаем поле _domainEvents из сериализации через SetIgnoreExtraElements
                    cm.SetIgnoreExtraElements(true);
                });
                Console.WriteLine("✅ [BsonConfiguration] Assignment класс зарегистрирован с исключением _domainEvents");
            }

            // 8. Настройка класса Patient с явным маппингом всех свойств
            if (!BsonClassMap.IsClassMapRegistered(typeof(Patient)))
            {
                BsonClassMap.RegisterClassMap<Patient>((Action<BsonClassMap<Patient>>)(cm =>
                {
                    cm.AutoMap();
                    
                    // Исключаем поле _domainEvents из сериализации через SetIgnoreExtraElements
                    cm.SetIgnoreExtraElements(true);
                    
                    // Явно маппим только свойства из Patient (не из базового класса)
                    cm.MapProperty(c => c.FullName);
                    cm.MapProperty(c => c.Birthday);
                    cm.MapProperty(c => c.CitizenshipInfo);
                    cm.MapProperty(c => c.Capable);
                    cm.MapProperty(c => c.Pension);
                    cm.MapProperty(c => c.Note);
                    cm.MapProperty(c => c.SoftDeleted);
                    cm.MapProperty(c => c.IsArchive);
                    // Маппим публичное свойство MedicalHistories с явной настройкой
                    cm.MapProperty(c => c.MedicalHistories)
                      .SetElementName("MedicalHistories");
                    
                    // Простая настройка Documents как ArrayOfArrays без сложных сериализаторов
                    cm.GetMemberMap(c => c.Documents)
                      .SetSerializer(new DictionaryInterfaceImplementerSerializer<Dictionary<DocumentType, object>>(
                          DictionaryRepresentation.ArrayOfArrays));
                }));
                Console.WriteLine("✅ [BsonConfiguration] Patient класс зарегистрирован с полным маппингом");
            }


            // Регистрируем кастомный сериализатор для Patient
            BsonSerializer.RegisterSerializer<Patient>(new PatientSerializer());
            Console.WriteLine("✅ [BsonConfiguration] PatientSerializer зарегистрирован");

            _isRegistered = true;
            Console.WriteLine("🎉 [BsonConfiguration] Полная BSON конфигурация завершена!");
        }
    }
}
