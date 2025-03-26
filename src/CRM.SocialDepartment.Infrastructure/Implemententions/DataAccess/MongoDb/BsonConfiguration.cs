using CRM.SocialDepartment.Domain.Entities.Patients;
using MongoDB.Bson.Serialization;

namespace CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb
{
    public static class BsonConfiguration
    {
        public static void RegisterMappings()
        {
            //BsonClassMap.RegisterClassMap<Capable>(cm =>
            //{
            //    cm.AutoMap();
            //    cm.MapProperty(c => c.CourtDecision).SetElementName("court_decision");
            //    cm.MapProperty(c => c.TrialDate).SetElementName("trialdate");
            //    cm.MapProperty(c => c.Guardian).SetElementName("guardian");
            //    cm.MapProperty(c => c.GuardianOrderAppointment).SetElementName("guardian_order_appointment");
            //});
        }
    }
}
