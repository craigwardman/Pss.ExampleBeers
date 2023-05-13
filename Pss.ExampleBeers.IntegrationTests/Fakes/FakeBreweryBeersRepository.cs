using Moq;
using Pss.ExampleBeers.Models.Interfaces;
using Pss.ExampleBeers.Models.Model.Beers;
using Pss.ExampleBeers.Models.Model.Breweries;
using Pss.ExampleBeers.MongoDB.Mongo;
using TestDataDefinitionFramework.Core;

namespace Pss.ExampleBeers.IntegrationTests.Fakes;

public class FakeBreweryBeersRepository
{
    public FakeBreweryBeersRepository()
    {
        Mock.Setup(m => m.GetBreweryBeersAsync())
            .ReturnsAsync(() => GetInMemoryBreweryBeers(null));

        Mock.Setup(m => m.GetBreweryBeersAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) => GetInMemoryBreweryBeers(id).FirstOrDefault());
    }

    public Mock<IBreweryBeersRepository> Mock { get; } = new();

    private static IReadOnlyList<BreweryWithBeers> GetInMemoryBreweryBeers(Guid? breweryId)
    {
        var breweryBeers = TestDataStore.Repository<BreweryBeer>(BreweryBeersCollection.Name).Items ?? Array.Empty<BreweryBeer>();
        var breweriesCollection = TestDataStore.Repository<Brewery>(BreweriesCollection.Name).Items ?? Array.Empty<Brewery>();
        var beersCollection = TestDataStore.Repository<Beer>(BeersCollection.Name).Items ?? Array.Empty<Beer>();

        var query =
            breweryBeers.Where(bb => breweryId == null || bb.BreweryId == breweryId)
                .Join(breweriesCollection, bb => bb.BreweryId, brewery => brewery.Id,
                    (bb, brewery) => new { brewery, bb.BeerId })
                .Join(beersCollection, bb => bb.BeerId, beer => beer.Id,
                    (bb, beer) => new { bb.brewery, beer })
                .GroupBy(ks => ks.brewery)
                .Select(vs => new { Brewery = vs.Key, Beers = vs.Select(bb => bb.beer).ToArray() });

        return query.Select(r => new BreweryWithBeers(r.Brewery, r.Beers)).ToArray();
    }
}