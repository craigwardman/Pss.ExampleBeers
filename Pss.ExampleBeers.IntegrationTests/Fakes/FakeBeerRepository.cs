using Moq;
using Pss.ExampleBeers.Models.Interfaces;
using Pss.ExampleBeers.Models.Model.Beers;
using Pss.ExampleBeers.MongoDB.Mongo;
using TestDataDefinitionFramework.Core;

namespace Pss.ExampleBeers.IntegrationTests.Fakes;

public class FakeBeerRepository
{
    public Mock<IBeerRepository> Mock { get; } = new();

    public FakeBeerRepository()
    {
        Mock.Setup(m => m.GetBeersAsync(It.IsAny<double?>(), It.IsAny<double?>()))
            .ReturnsAsync((double? gtAlcoholByVolume, double? ltAlcoholByValue) =>
                GetInMemoryBeers(gtAlcoholByVolume, ltAlcoholByValue));

        Mock.Setup(m => m.GetBeerAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) => GetInMemoryBeer(id));
    }

    private static IReadOnlyList<Beer> GetInMemoryBeers(double? gtAlcoholByVolume, double? ltAlcoholByValue)
    {
        return TestDataStore.Repository<Beer>(BeersCollection.Name).Items.Where(b =>
                (!ltAlcoholByValue.HasValue || b.PercentageAlcoholByVolume < ltAlcoholByValue.Value) &&
                (!gtAlcoholByVolume.HasValue || b.PercentageAlcoholByVolume > gtAlcoholByVolume.Value))
            .ToArray();
    }
    
    private static Beer? GetInMemoryBeer(Guid id)
    {
        return TestDataStore.Repository<Beer>(BeersCollection.Name).Items?
            .SingleOrDefault(b => b.Id == id);
    }
}