using Moq;
using Pss.ExampleBeers.Domain.Interfaces;
using Pss.ExampleBeers.Domain.Model.Breweries;
using Pss.ExampleBeers.MongoDB.Mongo;
using TestDataDefinitionFramework.Core;

namespace Pss.ExampleBeers.IntegrationTests.Fakes;

public class FakeBreweryRepository
{
    public FakeBreweryRepository()
    {
        Mock.Setup(m => m.GetBreweriesAsync())
            .ReturnsAsync(GetInMemoryBreweries);

        Mock.Setup(m => m.GetBreweryAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) => GetInMemoryBrewery(id));
    }

    public Mock<IBreweryRepository> Mock { get; } = new();

    private static IReadOnlyList<Brewery> GetInMemoryBreweries()
    {
        return TestDataStore.Repository<Brewery>(BreweriesCollection.Name).Items;
    }

    private static Brewery? GetInMemoryBrewery(Guid id)
    {
        return TestDataStore.Repository<Brewery>(BreweriesCollection.Name).Items
            .SingleOrDefault(b => b.Id == id);
    }
}