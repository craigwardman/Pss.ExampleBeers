using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Pss.ExampleBeers.Models.Model.Bars;

namespace Pss.ExampleBeers.MongoDB.Mongo;

public class BarBeersCollection : IBootstrapped
{
    public const string Name = "barBeers";

    public void Setup(IMongoDatabase database)
    {
        RegisterClassMaps();
        CreateCollections(database);
    }

    private static void RegisterClassMaps()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(BarBeer)))
            BsonClassMap.RegisterClassMap<BarBeer>(dataMap =>
            {
                dataMap.AutoMap();
                dataMap.SetIgnoreExtraElements(true);
            });
    }

    private static void CreateCollections(IMongoDatabase database)
    {
        if (!database.ListCollectionNames().ToList().Contains(Name)) database.CreateCollection(Name);
    }
}