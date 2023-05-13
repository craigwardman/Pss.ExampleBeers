using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Pss.ExampleBeers.Models.Interfaces;
using Pss.ExampleBeers.Models.Model.Bars;
using Pss.ExampleBeers.Models.Model.Beers;
using Pss.ExampleBeers.MongoDB.Mongo;

namespace Pss.ExampleBeers.MongoDB;

internal class BarBeersRepository : IBarBeersRepository
{
    private readonly IMongoConnection _mongoConnection;

    public BarBeersRepository(IMongoConnection mongoConnection)
    {
        _mongoConnection = mongoConnection ?? throw new ArgumentNullException(nameof(mongoConnection));
    }

    public async Task InsertAsync(BarBeer barBeer)
    {
        var collection = _mongoConnection.Database.GetCollection<BarBeer>(BarBeersCollection.Name);

        await collection.InsertOneAsync(barBeer);
    }

    public async Task<BarWithBeers?> GetBarBeersAsync(Guid barId)
    {
        var result = await GetBarBeersAsyncImpl(barId);

        return result.SingleOrDefault();
    }

    public Task<IReadOnlyList<BarWithBeers>> GetBarBeersAsync()
    {
        return GetBarBeersAsyncImpl(null);
    }

    private async Task<IReadOnlyList<BarWithBeers>> GetBarBeersAsyncImpl(Guid? barId)
    {
        var barBeers = _mongoConnection.Database.GetCollection<BarBeer>(BarBeersCollection.Name)
            .AsQueryable();
        var barsCollection =
            _mongoConnection.Database.GetCollection<Bar>(BarsCollection.Name).AsQueryable();
        var beersCollection = _mongoConnection.Database.GetCollection<Beer>(BeersCollection.Name).AsQueryable();

        var query =
            barBeers.Where(bb => barId == null || bb.BarId == barId)
                .Join(barsCollection, bb => bb.BarId, bar => bar.Id,
                    (bb, bar) => new { bar, bb.BeerId })
                .Join(beersCollection, bb => bb.BeerId, beer => beer.Id,
                    (bb, beer) => new { bb.bar, beer })
                .GroupBy(ks => ks.bar)
                .Select(vs => new { Bar = vs.Key, Beers = vs.Select(bb => bb.beer).ToArray() });

        var result = await query.ToListAsync();

        return result != null
            ? result.Select(r => new BarWithBeers(r.Bar, r.Beers)).ToArray()
            : Array.Empty<BarWithBeers>();
    }
}