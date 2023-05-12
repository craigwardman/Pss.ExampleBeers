using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Pss.ExampleBeers.Domain.Model.Beers;

namespace Pss.ExampleBeers.MongoDB.Mongo;

public class BeersCollection : IBootstrapped
{
    public const string Name = "beers";
    
    public void Setup(IMongoDatabase database)
    {
        RegisterClassMaps();
        CreateCollections(database);
        CreateIndices(database);
    }
    
    private static void RegisterClassMaps()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(Beer)))
        {
            BsonClassMap.RegisterClassMap<Beer>(dataMap =>
            {
                dataMap.AutoMap();
                dataMap.MapIdProperty(m => m.Id);
            });
        }
    }

    private static void CreateIndices(IMongoDatabase database)
    {
        database.GetCollection<Beer>(Name).Indexes.CreateOne(
            new CreateIndexModel<Beer>(
                Builders<Beer>.IndexKeys
                    .Ascending(l => l.PercentageAlcoholByVolume))
        );
    }

    private static void CreateCollections(IMongoDatabase database)
    {
        if (!database.ListCollectionNames().ToList().Contains(Name))
        {
            database.CreateCollection(Name);
        }
    }
}