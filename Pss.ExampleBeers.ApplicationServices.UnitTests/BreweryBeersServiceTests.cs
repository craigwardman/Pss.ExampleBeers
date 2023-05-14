using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using Pss.ExampleBeers.Models.Interfaces;
using Pss.ExampleBeers.Models.Model.Breweries;
using Pss.ExampleBeers.Models.Model.Beers;

namespace Pss.ExampleBeers.ApplicationServices.UnitTests;

[TestFixture]
public class BreweryBeersServiceTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IBreweryBeersRepository> _breweryBeerRepositoryMock = new();
    private readonly Mock<IBreweryService> _breweryServiceMock = new();
    private readonly Mock<IBeerService> _beerServiceMock = new();
    
    public BreweryBeersServiceTests()
    {
        _fixture.Customize(new AutoMoqCustomization());
    }

    [SetUp]
    public void SetUp()
    {
        _breweryBeerRepositoryMock.Reset();
        _breweryServiceMock.Reset();
        _beerServiceMock.Reset();
    }
    
    [Test]
    public void Ctor_NullDependencies_Throws()
    {
        new GuardClauseAssertion(_fixture)
            .Verify(GetDefaultSut().GetType().GetConstructors());
    }

    [Test]
    public async Task LinkBeerAsync_BreweryAndBeerFound_InsertsLink()
    {
        var brewery = _fixture.Create<Brewery>();
        var beer = _fixture.Create<Beer>();
        _breweryServiceMock.Setup(s => s.GetAsync(brewery.Id)).ReturnsAsync(brewery);
        _beerServiceMock.Setup(s => s.GetAsync(beer.Id)).ReturnsAsync(beer);

        var result = await GetDefaultSut().LinkBeerAsync(brewery.Id, beer.Id);

        using (new AssertionScope())
        {
            _breweryBeerRepositoryMock.Verify(r => r.InsertAsync(It.Is<BreweryBeer>(bb => bb.BreweryId == brewery.Id && bb.BeerId == beer.Id)));
            result.brewery.Should().Be(brewery);
            result.beer.Should().Be(beer);
        }
    }
    
    [Test]
    public async Task LinkBeerAsync_BreweryNotFound_DoesNotInsertLink()
    {
        var beer = _fixture.Create<Beer>();
        _breweryServiceMock.Setup(s => s.GetAsync(It.IsAny<Guid>())).ReturnsAsync((Brewery?)null);
        _beerServiceMock.Setup(s => s.GetAsync(beer.Id)).ReturnsAsync(beer);

        var result = await GetDefaultSut().LinkBeerAsync(Guid.NewGuid(), beer.Id);

        using (new AssertionScope())
        {
            _breweryBeerRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<BreweryBeer>()), Times.Never);
            result.brewery.Should().BeNull();
            result.beer.Should().Be(beer);
        }
    }
    
    [Test]
    public async Task LinkBeerAsync_BeerNotFound_DoesNotInsertLink()
    {
        var brewery = _fixture.Create<Brewery>();
        _breweryServiceMock.Setup(s => s.GetAsync(brewery.Id)).ReturnsAsync(brewery);
        _beerServiceMock.Setup(s => s.GetAsync(It.IsAny<Guid>())).ReturnsAsync((Beer?)null);

        var result = await GetDefaultSut().LinkBeerAsync(brewery.Id, Guid.NewGuid());

        using (new AssertionScope())
        {
            _breweryBeerRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<BreweryBeer>()), Times.Never);
            result.brewery.Should().Be(brewery);
            result.beer.Should().BeNull();
        }
    }

    [Test]
    public async Task GetBreweryBeersAsync_WhenCalled_GetsRecordFromRepository()
    {
        var record = _fixture.Create<BreweryWithBeers>();
        _breweryBeerRepositoryMock.Setup(r => r.GetBreweryBeersAsync(record.Brewery.Id)).ReturnsAsync(record);

        var result = await GetDefaultSut().GetBreweryBeersAsync(record.Brewery.Id);
        
        result.Should().Be(record);
    }
    
    [Test]
    public async Task GetBreweryBeersAsync_WhenCalled_GetsRecordsFromRepository()
    {
        var records = _fixture.CreateMany<BreweryWithBeers>().ToArray();
        _breweryBeerRepositoryMock.Setup(r => r.GetBreweryBeersAsync()).ReturnsAsync(records);

        var results = await GetDefaultSut().GetBreweryBeersAsync();
        
        results.Should().BeEquivalentTo(records);
    }
    
    private BreweryBeersService GetDefaultSut() =>
        new(_breweryServiceMock.Object, _beerServiceMock.Object, _breweryBeerRepositoryMock.Object);
}