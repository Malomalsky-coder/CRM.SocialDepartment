namespace CRM.SocialDepartment.WebApp.Settings
{
    public class MongoDbSetting
    {
        public string Name { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public string ConnectionString => $"mongodb://{Host}:{Port}";
    }
}
