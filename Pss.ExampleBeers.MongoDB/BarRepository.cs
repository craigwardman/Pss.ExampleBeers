using MongoDB.Driver;
using Pss.ExampleBeers.Models.Interfaces;
using Pss.ExampleBeers.Models.Model.Bars;
using Pss.ExampleBeers.MongoDB.Mongo;

namespace Pss.ExampleBeers.MongoDB;

internal class BarRepository : IBarRepository
{
    private readonly IMongoConnection _mongoConnection;

    public BarRepository(IMongoConnection mongoConnection)
    {
        _mongoConnection = mongoConnection ?? throw new ArgumentNullException(nameof(mongoConnection));
    }
    
    public async Task<IReadOnlyList<Bar>> GetBarsAsync()
    {
        var collection = _mongoConnection.Database.GetCollection<Bar>(BarsCollection.Name);
        var result = await collection.Find(FilterDefinition<Bar>.Empty)
            .ToListAsync();

        return result;
    }

    public async Task UpsertBarAsync(Bar brewery)
    {
        var collection = _mongoConnection.Database.GetCollection<Bar>(BarsCollection.Name);

        await collection.ReplaceOneAsync(b => b.Id == brewery.Id, brewery, new ReplaceOptions()
        {
            IsUpsert = true
        });
    }

    public async Task<Bar?> GetBarAsync(Guid id)
    {
        var collection = _mongoConnection.Database.GetCollection<Bar>(BarsCollection.Name);
        var result = await collection.Find(b =>
                b.Id == id)
            .SingleOrDefaultAsync();

        return result;
    }
}