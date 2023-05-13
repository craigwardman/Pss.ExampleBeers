using MongoDB.Driver;
using Pss.ExampleBeers.Models.Interfaces;
using Pss.ExampleBeers.Models.Model.Breweries;
using Pss.ExampleBeers.MongoDB.Mongo;

namespace Pss.ExampleBeers.MongoDB;

internal class BreweryRepository : IBreweryRepository
{
    private readonly IMongoConnection _mongoConnection;

    public BreweryRepository(IMongoConnection mongoConnection)
    {
        _mongoConnection = mongoConnection ?? throw new ArgumentNullException(nameof(mongoConnection));
    }
    
    public async Task<IReadOnlyList<Brewery>> GetBreweriesAsync()
    {
        var collection = _mongoConnection.Database.GetCollection<Brewery>(BreweriesCollection.Name);
        var result = await collection.Find(FilterDefinition<Brewery>.Empty)
            .ToListAsync();

        return result;
    }

    public async Task UpsertBreweryAsync(Brewery brewery)
    {
        var collection = _mongoConnection.Database.GetCollection<Brewery>(BreweriesCollection.Name);

        await collection.ReplaceOneAsync(b => b.Id == brewery.Id, brewery, new ReplaceOptions()
        {
            IsUpsert = true
        });
    }

    public async Task<Brewery?> GetBreweryAsync(Guid id)
    {
        var collection = _mongoConnection.Database.GetCollection<Brewery>(BreweriesCollection.Name);
        var result = await collection.Find(b =>
                b.Id == id)
            .SingleOrDefaultAsync();

        return result;
    }
}