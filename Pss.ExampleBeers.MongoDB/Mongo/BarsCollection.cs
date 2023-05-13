using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Pss.ExampleBeers.Models.Model.Bars;

namespace Pss.ExampleBeers.MongoDB.Mongo;

public class BarsCollection : IBootstrapped
{
    public const string Name = "bars";
    
    public void Setup(IMongoDatabase database)
    {
        RegisterClassMaps();
        CreateCollections(database);
    }
    
    private static void RegisterClassMaps()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(Bar)))
        {
            BsonClassMap.RegisterClassMap<Bar>(dataMap =>
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