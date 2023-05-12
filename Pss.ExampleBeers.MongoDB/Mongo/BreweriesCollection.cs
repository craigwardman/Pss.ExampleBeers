using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Pss.ExampleBeers.Domain.Model.Breweries;

namespace Pss.ExampleBeers.MongoDB.Mongo;

public class BreweriesCollection : IBootstrapped
{
    public const string Name = "breweries";
    
    public void Setup(IMongoDatabase database)
    {
        RegisterClassMaps();
        CreateCollections(database);
    }
    
    private static void RegisterClassMaps()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(Brewery)))
        {
            BsonClassMap.RegisterClassMap<Brewery>(dataMap =>
            {
                dataMap.AutoMap();
                dataMap.MapIdProperty(m => m.Id);
            });
        }
    }

    private static void CreateCollections(IMongoDatabase database)
    {
        if (!database.ListCollectionNames().ToList().Contains(Name))
        {
            database.CreateCollection(Name);
        }
    }
}