using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Pss.ExampleBeers.Api.Controllers;
using Pss.ExampleBeers.Api.Models;
using Pss.ExampleBeers.ApplicationServices;
using Pss.ExampleBeers.Models.Model.Beers;

namespace Pss.ExampleBeers.Api.UnitTests.Controllers;

[TestFixture]
public class BeerControllerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IBeerService> _beerServiceMock = new();

    public BeerControllerTests()
    {
        _fixture.Customize(new AutoMoqCustomization());
    }

    [SetUp]
    public void SetUp()
    {
        _beerServiceMock.Reset();
    }

    [Test]
    public void Ctor_NullDependencies_Throws()
    {
        new GuardClauseAssertion(_fixture)
            .Verify(GetDefaultSut().GetType().GetConstructors());
    }
    
    [Test]
    public async Task Post_WhenCalled_CreatesBeerAndReturnsCreated()
    {
        var request = _fixture.Create<BeerRequestModel>();
        var createdBeer = _fixture.Create<Beer>();
        _beerServiceMock.Setup(s => s.CreateAsync(request.Name, request.PercentageAlcoholByVolume))
            .ReturnsAsync(createdBeer);
        
        var result = await GetDefaultSut().Post(request);

        result.Should().BeOfType<CreatedAtActionResult>()
            .Which.Value.Should().Be(createdBeer.Id);
    }

    [Test]
    public async Task Get_WhenNoBeersExist_ReturnsNoContent()
    {
        _beerServiceMock.Setup(s => s.GetAsync(It.IsAny<double?>(), It.IsAny<double?>()))
            .ReturnsAsync(Array.Empty<Beer>());

        var result = await GetDefaultSut().Get(_fixture.Create<double?>(), _fixture.Create<double?>());

        result.Should().BeOfType<NoContentResult>();
    }

    [Test]
    public async Task Get_WhenBeersExist_ReturnsOkObject()
    {
        var beers = _fixture.CreateMany<Beer>().ToArray();
        _beerServiceMock.Setup(s => s.GetAsync(It.IsAny<double?>(), It.IsAny<double?>())).ReturnsAsync(beers);

        var result = await GetDefaultSut().Get(_fixture.Create<double?>(), _fixture.Create<double?>());

        result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(beers);
    }
    
    [Test]
    public async Task Get_WhenBeerDoesNotExist_ReturnsNotFoundResult()
    {
        var beerId = Guid.NewGuid();
        _beerServiceMock.Setup(s => s.GetAsync(beerId))
            .ReturnsAsync((Beer?)null);

        var result = await GetDefaultSut().Get(beerId);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Test]
    public async Task Get_WhenBeerExists_ReturnsOkObject()
    {
        var beerId = Guid.NewGuid();
        var beer = _fixture.Create<Beer>();
        _beerServiceMock.Setup(s => s.GetAsync(beerId)).ReturnsAsync(beer);

        var result = await GetDefaultSut().Get(beerId);

        result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(beer);
    }
    
    [Test]
    public async Task Put_WhenUpdatedBeerExists_ReturnsOkObject()
    {
        var request = _fixture.Create<BeerRequestModel>();
        var beerId = Guid.NewGuid();
        var updatedBeer = _fixture.Create<Beer>();
        _beerServiceMock.Setup(s => s.UpdateAsync(beerId, request.Name, request.PercentageAlcoholByVolume))
            .ReturnsAsync(updatedBeer);
        
        var result = await GetDefaultSut().Put(beerId, request);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(updatedBeer);
    }
    
    [Test]
    public async Task Put_WhenUpdatedBeerDoesNotExist_ReturnsNotFound()
    {
        var request = _fixture.Create<BeerRequestModel>();
        var beerId = Guid.NewGuid();
        _beerServiceMock.Setup(s => s.UpdateAsync(beerId, request.Name, request.PercentageAlcoholByVolume))
            .ReturnsAsync((Beer?)null);
        
        var result = await GetDefaultSut().Put(beerId, request);

        result.Should().BeOfType<NotFoundResult>();
    }
    
    private BeerController GetDefaultSut() => new(_beerServiceMock.Object);
}