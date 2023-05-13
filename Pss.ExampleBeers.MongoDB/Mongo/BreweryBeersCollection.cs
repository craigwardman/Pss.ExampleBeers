using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Pss.ExampleBeers.Models.Model.Breweries;

namespace Pss.ExampleBeers.MongoDB.Mongo;

public class BreweryBeersCollection : IBootstrapped
{
    public const string Name = "breweryBeers";
    
    public void Setup(IMongoDatabase database)
    {
        RegisterClassMaps();
        CreateCollections(database);
    }
    
    private static void RegisterClassMaps()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(BreweryBeer)))
        {
            BsonClassMap.RegisterClassMap<BreweryBeer>(dataMap =>
            {
                dataMap.AutoMap();
                dataMap.SetIgnoreExtraElements(true);
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