using Moq;
using Pss.ExampleBeers.Models.Interfaces;
using Pss.ExampleBeers.Models.Model.Bars;
using Pss.ExampleBeers.Models.Model.Beers;
using Pss.ExampleBeers.MongoDB.Mongo;
using TestDataDefinitionFramework.Core;

namespace Pss.ExampleBeers.IntegrationTests.Fakes;

public class FakeBarBeersRepository
{
    public FakeBarBeersRepository()
    {
        Mock.Setup(m => m.GetBarBeersAsync())
            .ReturnsAsync(() => GetInMemoryBarBeers(null));

        Mock.Setup(m => m.GetBarBeersAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) => GetInMemoryBarBeers(id).FirstOrDefault());
    }

    public Mock<IBarBeersRepository> Mock { get; } = new();

    private static IReadOnlyList<BarWithBeers> GetInMemoryBarBeers(Guid? barId)
    {
        var barBeers = TestDataStore.Repository<BarBeer>(BarBeersCollection.Name).Items ?? Array.Empty<BarBeer>();
        var barsCollection = TestDataStore.Repository<Bar>(BarsCollection.Name).Items ?? Array.Empty<Bar>();
        var beersCollection = TestDataStore.Repository<Beer>(BeersCollection.Name).Items ?? Array.Empty<Beer>();

        var query =
            barBeers.Where(bb => barId == null || bb.BarId == barId)
                .Join(barsCollection, bb => bb.BarId, bar => bar.Id,
                    (bb, bar) => new { bar, bb.BeerId })
                .Join(beersCollection, bb => bb.BeerId, beer => beer.Id,
                    (bb, beer) => new { bb.bar, beer })
                .GroupBy(ks => ks.bar)
                .Select(vs => new { Bar = vs.Key, Beers = vs.Select(bb => bb.beer).ToArray() });

        return query.Select(r => new BarWithBeers(r.Bar, r.Beers)).ToArray();
    }
}