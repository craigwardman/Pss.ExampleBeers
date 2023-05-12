using MongoDB.Driver;
using Pss.ExampleBeers.Domain.Interfaces;
using Pss.ExampleBeers.Domain.Model.Beers;
using Pss.ExampleBeers.MongoDB.Mongo;

namespace Pss.ExampleBeers.MongoDB;

internal class BeerRepository : IBeerRepository
{
    private readonly IMongoConnection _mongoConnection;

    public BeerRepository(IMongoConnection mongoConnection)
    {
        _mongoConnection = mongoConnection ?? throw new ArgumentNullException(nameof(mongoConnection));
    }
    
    public async Task<IReadOnlyList<Beer>> GetBeersAsync(double? gtAlcoholByVolume, double? ltAlcoholByValue)
    {
        var collection = _mongoConnection.Database.GetCollection<Beer>(BeersCollection.Name);
        var result = await collection.Find(b =>
                (!ltAlcoholByValue.HasValue || b.PercentageAlcoholByVolume < ltAlcoholByValue.Value) &&
                (!gtAlcoholByVolume.HasValue || b.PercentageAlcoholByVolume > gtAlcoholByVolume.Value))
            .ToListAsync();

        return result;
    }

    public async Task UpsertBeerAsync(Beer beer)
    {
        var collection = _mongoConnection.Database.GetCollection<Beer>(BeersCollection.Name);

        await collection.ReplaceOneAsync(b => b.Id == beer.Id, beer, new ReplaceOptions()
        {
            IsUpsert = true
        });
    }

    public async Task<Beer?> GetBeerAsync(Guid id)
    {
        var collection = _mongoConnection.Database.GetCollection<Beer>(BeersCollection.Name);
        var result = await collection.Find(b =>
                b.Id == id)
            .SingleOrDefaultAsync();

        return result;
    }
}