using Moq;
using Pss.ExampleBeers.Domain.Interfaces;
using Pss.ExampleBeers.Domain.Model.Bars;
using Pss.ExampleBeers.MongoDB.Mongo;
using TestDataDefinitionFramework.Core;

namespace Pss.ExampleBeers.IntegrationTests.Fakes;

public class FakeBarRepository
{
    public FakeBarRepository()
    {
        Mock.Setup(m => m.GetBarsAsync())
            .ReturnsAsync(GetInMemoryBars);

        Mock.Setup(m => m.GetBarAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) => GetInMemoryBar(id));
    }

    public Mock<IBarRepository> Mock { get; } = new();

    private static IReadOnlyList<Bar> GetInMemoryBars()
    {
        return TestDataStore.Repository<Bar>(BarsCollection.Name).Items;
    }

    private static Bar? GetInMemoryBar(Guid id)
    {
        return TestDataStore.Repository<Bar>(BarsCollection.Name).Items
            .SingleOrDefault(b => b.Id == id);
    }
}