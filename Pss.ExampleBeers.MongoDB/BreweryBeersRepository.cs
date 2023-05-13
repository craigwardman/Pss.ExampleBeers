using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Pss.ExampleBeers.Models.Interfaces;
using Pss.ExampleBeers.Models.Model.Beers;
using Pss.ExampleBeers.Models.Model.Breweries;
using Pss.ExampleBeers.MongoDB.Mongo;

namespace Pss.ExampleBeers.MongoDB;

internal class BreweryBeersRepository : IBreweryBeersRepository
{
    private readonly IMongoConnection _mongoConnection;

    public BreweryBeersRepository(IMongoConnection mongoConnection)
    {
        _mongoConnection = mongoConnection ?? throw new ArgumentNullException(nameof(mongoConnection));
    }

    public async Task InsertAsync(BreweryBeer breweryBeer)
    {
        var collection = _mongoConnection.Database.GetCollection<BreweryBeer>(BreweryBeersCollection.Name);

        await collection.InsertOneAsync(breweryBeer);
    }

    public async Task<BreweryWithBeers?> GetBreweryBeersAsync(Guid breweryId)
    {
        var result = await GetBreweryBeersAsyncImpl(breweryId);

        return result.SingleOrDefault();
    }

    public Task<IReadOnlyList<BreweryWithBeers>> GetBreweryBeersAsync()
    {
        return GetBreweryBeersAsyncImpl(null);
    }

    private async Task<IReadOnlyList<BreweryWithBeers>> GetBreweryBeersAsyncImpl(Guid? breweryId)
    {
        var breweryBeers = _mongoConnection.Database.GetCollection<BreweryBeer>(BreweryBeersCollection.Name)
            .AsQueryable();
        var breweriesCollection =
            _mongoConnection.Database.GetCollection<Brewery>(BreweriesCollection.Name).AsQueryable();
        var beersCollection = _mongoConnection.Database.GetCollection<Beer>(BeersCollection.Name).AsQueryable();

        var query =
            breweryBeers.Where(bb => breweryId == null || bb.BreweryId == breweryId)
                .Join(breweriesCollection, bb => bb.BreweryId, brewery => brewery.Id,
                    (bb, brewery) => new { brewery, bb.BeerId })
                .Join(beersCollection, bb => bb.BeerId, beer => beer.Id,
                    (bb, beer) => new { bb.brewery, beer })
                .GroupBy(ks => ks.brewery)
                .Select(vs => new { Brewery = vs.Key, Beers = vs.Select(bb => bb.beer).ToArray() });

        var result = await query.ToListAsync();
        
        return result != null ? 
            result.Select(r => new BreweryWithBeers(r.Brewery, r.Beers)).ToArray() : 
            Array.Empty<BreweryWithBeers>();
    }
}