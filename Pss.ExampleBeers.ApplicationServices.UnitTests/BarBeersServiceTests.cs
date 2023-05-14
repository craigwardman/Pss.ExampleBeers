using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using Pss.ExampleBeers.Models.Interfaces;
using Pss.ExampleBeers.Models.Model.Bars;
using Pss.ExampleBeers.Models.Model.Beers;

namespace Pss.ExampleBeers.ApplicationServices.UnitTests;

[TestFixture]
public class BarBeersServiceTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IBarBeersRepository> _barBeerRepositoryMock = new();
    private readonly Mock<IBarService> _barServiceMock = new();
    private readonly Mock<IBeerService> _beerServiceMock = new();
    
    public BarBeersServiceTests()
    {
        _fixture.Customize(new AutoMoqCustomization());
    }

    [SetUp]
    public void SetUp()
    {
        _barBeerRepositoryMock.Reset();
        _barServiceMock.Reset();
        _beerServiceMock.Reset();
    }
    
    [Test]
    public void Ctor_NullDependencies_Throws()
    {
        new GuardClauseAssertion(_fixture)
            .Verify(GetDefaultSut().GetType().GetConstructors());
    }

    [Test]
    public async Task LinkBeerAsync_BarAndBeerFound_InsertsLink()
    {
        var bar = _fixture.Create<Bar>();
        var beer = _fixture.Create<Beer>();
        _barServiceMock.Setup(s => s.GetAsync(bar.Id)).ReturnsAsync(bar);
        _beerServiceMock.Setup(s => s.GetAsync(beer.Id)).ReturnsAsync(beer);

        var result = await GetDefaultSut().LinkBeerAsync(bar.Id, beer.Id);

        using (new AssertionScope())
        {
            _barBeerRepositoryMock.Verify(r => r.InsertAsync(It.Is<BarBeer>(bb => bb.BarId == bar.Id && bb.BeerId == beer.Id)));
            result.bar.Should().Be(bar);
            result.beer.Should().Be(beer);
        }
    }
    
    [Test]
    public async Task LinkBeerAsync_BarNotFound_DoesNotInsertLink()
    {
        var beer = _fixture.Create<Beer>();
        _barServiceMock.Setup(s => s.GetAsync(It.IsAny<Guid>())).ReturnsAsync((Bar?)null);
        _beerServiceMock.Setup(s => s.GetAsync(beer.Id)).ReturnsAsync(beer);

        var result = await GetDefaultSut().LinkBeerAsync(Guid.NewGuid(), beer.Id);

        using (new AssertionScope())
        {
            _barBeerRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<BarBeer>()), Times.Never);
            result.bar.Should().BeNull();
            result.beer.Should().Be(beer);
        }
    }
    
    [Test]
    public async Task LinkBeerAsync_BeerNotFound_DoesNotInsertLink()
    {
        var bar = _fixture.Create<Bar>();
        _barServiceMock.Setup(s => s.GetAsync(bar.Id)).ReturnsAsync(bar);
        _beerServiceMock.Setup(s => s.GetAsync(It.IsAny<Guid>())).ReturnsAsync((Beer?)null);

        var result = await GetDefaultSut().LinkBeerAsync(bar.Id, Guid.NewGuid());

        using (new AssertionScope())
        {
            _barBeerRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<BarBeer>()), Times.Never);
            result.bar.Should().Be(bar);
            result.beer.Should().BeNull();
        }
    }

    [Test]
    public async Task GetBarBeersAsync_WhenCalled_GetsRecordFromRepository()
    {
        var record = _fixture.Create<BarWithBeers>();
        _barBeerRepositoryMock.Setup(r => r.GetBarBeersAsync(record.Bar.Id)).ReturnsAsync(record);

        var result = await GetDefaultSut().GetBarBeersAsync(record.Bar.Id);
        
        result.Should().Be(record);
    }
    
    [Test]
    public async Task GetBarBeersAsync_WhenCalled_GetsRecordsFromRepository()
    {
        var records = _fixture.CreateMany<BarWithBeers>().ToArray();
        _barBeerRepositoryMock.Setup(r => r.GetBarBeersAsync()).ReturnsAsync(records);

        var results = await GetDefaultSut().GetBarBeersAsync();
        
        results.Should().BeEquivalentTo(records);
    }
    
    private BarBeersService GetDefaultSut() =>
        new(_barServiceMock.Object, _beerServiceMock.Object, _barBeerRepositoryMock.Object);
}