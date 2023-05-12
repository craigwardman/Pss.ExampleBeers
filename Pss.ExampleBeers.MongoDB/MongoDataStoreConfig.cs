namespace Pss.ExampleBeers.MongoDB
{
    public class MongoDataStoreConfig
    {
        public const string Section = "Mongo";
        
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
    }
}