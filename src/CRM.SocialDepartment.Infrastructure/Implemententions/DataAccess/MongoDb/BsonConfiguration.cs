using CRM.SocialDepartment.Domain.Entities.Patients;
using MongoDB.Bson.Serialization;

namespace CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb
{
    public static class BsonConfiguration
    {
        public static void RegisterMappings()
        {
            //Вариант №1
            //BsonClassMap.RegisterClassMap<Capable>(cm =>
            //{
            //    cm.AutoMap();
            //    cm.MapConstructor(typeof(Capable).GetConstructor(new[] { typeof(string), typeof(DateTime), typeof(string), typeof(string) }));
            //});

            //Вариант №2
            //BsonClassMap.RegisterClassMap<Capable>(cm =>
            //{
            //    cm.AutoMap();
            //    cm.MapCreator(c => new Capable(
            //        c.CourtDecision,
            //        c.TrialDate,
            //        c.Guardian,
            //        c.GuardianOrderAppointment
            //    ));

            //    cm.MapProperty(c => c.CourtDecision).SetElementName("court_decision");
            //    cm.MapProperty(c => c.TrialDate).SetElementName("trialdate");
            //    cm.MapProperty(c => c.Guardian).SetElementName("guardian");
            //    cm.MapProperty(c => c.GuardianOrderAppointment).SetElementName("guardian_order_appointment");
            //});
        }
    }
}
