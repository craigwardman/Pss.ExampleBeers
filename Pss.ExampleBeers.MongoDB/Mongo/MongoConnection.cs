using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Pss.ExampleBeers.MongoDB.Mongo
{
    internal class MongoConnection : IMongoConnection
    {
        public MongoConnection(IOptions<MongoDataStoreConfig> config, IEnumerable<IBootstrapped> bootstrappedItems)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            var settings = MongoClientSettings.FromUrl(new MongoUrl(config.Value.ConnectionString));
            var client = new MongoClient(settings);

            Database = client.GetDatabase(config.Value.DatabaseName);

            foreach (var bootstrapped in bootstrappedItems ?? Array.Empty<IBootstrapped>())
            {
                bootstrapped.Setup(Database);
            }
        }

        public IMongoDatabase Database { get; }
    }
}