using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using Pss.ExampleBeers.Models.Interfaces;
using Pss.ExampleBeers.Models.Model.Beers;

namespace Pss.ExampleBeers.ApplicationServices.UnitTests;

[TestFixture]
public class BeerServiceTests
{
    [SetUp]
    public void SetUp()
    {
        _beerRepositoryMock.Reset();
    }

    private readonly Fixture _fixture = new();
    private readonly Mock<IBeerRepository> _beerRepositoryMock = new();

    public BeerServiceTests()
    {
        _fixture.Customize(new AutoMoqCustomization());
    }

    [Test]
    public void Ctor_NullDependencies_Throws()
    {
        new GuardClauseAssertion(_fixture)
            .Verify(GetDefaultSut().GetType().GetConstructors());
    }

    [Test]
    public async Task CreateAsync_WhenCalled_InsertsNewBeer()
    {
        var inputs = new
        {
            Name = _fixture.Create<string>(),
            PercentageAlcoholByVolume = _fixture.Create<double>()
        };

        var result = await GetDefaultSut().CreateAsync(inputs.Name, inputs.PercentageAlcoholByVolume);

        using (new AssertionScope())
        {
            result.Should().BeEquivalentTo(inputs);
            _beerRepositoryMock.Verify(r => r.UpsertBeerAsync(result));
        }
    }
    
    [Test]
    public async Task GetAsync_WhenCalled_GetsRecordFromRepository()
    {
        var record = _fixture.Create<Beer>();
        _beerRepositoryMock.Setup(r => r.GetBeerAsync(record.Id)).ReturnsAsync(record);

        var result = await GetDefaultSut().GetAsync(record.Id);
        
        result.Should().Be(record);
    }
    
    [Test]
    public async Task GetAsync_WhenCalled_GetsRecordsFromRepository()
    {
        var records = _fixture.CreateMany<Beer>().ToArray();
        _beerRepositoryMock.Setup(r => r.GetBeersAsync(It.IsAny<double?>(), It.IsAny<double?>())).ReturnsAsync(records);

        var results = await GetDefaultSut().GetAsync(_fixture.Create<double?>(), _fixture.Create<double?>());
        
        results.Should().BeEquivalentTo(records);
    }
    
    [Test]
    public async Task UpdateAsync_WhenCalledForExistingBeer_UpdatesBeer()
    {
        var existingBeer = _fixture.Create<Beer>();
        var inputs = new
        {
            existingBeer.Id,
            Name = _fixture.Create<string>(),
            PercentageAlcoholByVolume = _fixture.Create<double>()
        };
        _beerRepositoryMock.Setup(r => r.GetBeerAsync(existingBeer.Id)).ReturnsAsync(existingBeer);

        var result = await GetDefaultSut().UpdateAsync(inputs.Id, inputs.Name, inputs.PercentageAlcoholByVolume);

        using (new AssertionScope())
        {
            result.Should().BeEquivalentTo(inputs);
            _beerRepositoryMock.Verify(r => r.UpsertBeerAsync(result!));
        }
    }

    [Test] public async Task UpdateAsync_WhenCalledForNonExistentBeer_ReturnsNull()
    {
        var inputs = new
        {
            Id = Guid.NewGuid(),
            Name = _fixture.Create<string>(),
            PercentageAlcoholByVolume = _fixture.Create<double>()
        };
        _beerRepositoryMock.Setup(r => r.GetBeerAsync(It.IsAny<Guid>())).ReturnsAsync((Beer?)null);

        var result = await GetDefaultSut().UpdateAsync(inputs.Id, inputs.Name, inputs.PercentageAlcoholByVolume);

        using (new AssertionScope())
        {
            result.Should().BeNull();
            _beerRepositoryMock.Verify(r => r.UpsertBeerAsync(It.IsAny<Beer>()), Times.Never);
        }
    }

    private BeerService GetDefaultSut() => new(_beerRepositoryMock.Object);
}