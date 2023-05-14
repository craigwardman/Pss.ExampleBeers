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
using Pss.ExampleBeers.Models.Model.Breweries;

namespace Pss.ExampleBeers.Api.UnitTests.Controllers;

[TestFixture]
public class BreweryBeersControllerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IBreweryBeersService> _breweryBeersServiceMock = new();

    public BreweryBeersControllerTests()
    {
        _fixture.Customize(new AutoMoqCustomization());
    }

    [SetUp]
    public void SetUp()
    {
        _breweryBeersServiceMock.Reset();
    }

    [Test]
    public void Ctor_NullDependencies_Throws()
    {
        new GuardClauseAssertion(_fixture)
            .Verify(GetDefaultSut().GetType().GetConstructors());
    }

    [Test]
    public async Task Post_WhenCalled_LinksBeer()
    {
        var request = _fixture.Create<BreweryBeerLinkModel>();

        await GetDefaultSut().Post(request);
        
        _breweryBeersServiceMock.Verify(s => s.LinkBeerAsync(request.BreweryId, request.BeerId));
    }
    
    [Test]
    public async Task Post_WhenCalledWithNonExistentBeerId_ReturnsBadRequest()
    {
        _breweryBeersServiceMock.Setup(s => s.LinkBeerAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync((_fixture.Create<Brewery>(), null));

        var result = await GetDefaultSut().Post(_fixture.Create<BreweryBeerLinkModel>());

        result.Should().BeOfType<BadRequestObjectResult>();
    }
    
    [Test]
    public async Task Post_WhenCalledWithNonExistentBreweryId_ReturnsBadRequest()
    {
        _breweryBeersServiceMock.Setup(s => s.LinkBeerAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync((null, _fixture.Create<Beer>()));

        var result = await GetDefaultSut().Post(_fixture.Create<BreweryBeerLinkModel>());

        result.Should().BeOfType<BadRequestObjectResult>();
    }
    
    [Test]
    public async Task Post_WhenLinkSuccessful_ReturnsNoContent()
    {
        _breweryBeersServiceMock.Setup(s => s.LinkBeerAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync((_fixture.Create<Brewery>(), _fixture.Create<Beer>()));

        var result = await GetDefaultSut().Post(_fixture.Create<BreweryBeerLinkModel>());

        result.Should().BeOfType<NoContentResult>();
    }

    [Test]
    public async Task Get_WhenBreweryWithBeersExists_ReturnOkObject()
    {
        var breweryId = Guid.NewGuid();
        var breweryWithBeers = _fixture.Create<BreweryWithBeers>();
        _breweryBeersServiceMock.Setup(s => s.GetBreweryBeersAsync(breweryId))
            .ReturnsAsync(breweryWithBeers);
        
        var result = await GetDefaultSut().Get(breweryId);

        result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(breweryWithBeers);
    }
    
    [Test]
    public async Task Get_WhenBreweryWithBeersDoesNotExist_ReturnNotFound()
    {
        var breweryId = Guid.NewGuid();
        _breweryBeersServiceMock.Setup(s => s.GetBreweryBeersAsync(breweryId))
            .ReturnsAsync((BreweryWithBeers?)null);
        
        var result = await GetDefaultSut().Get(breweryId);

        result.Should().BeOfType<NotFoundResult>();
    }
    
    [Test]
    public async Task Get_WhenBreweriesWithBeersExists_ReturnOkObject()
    {
        var breweriesWithBeers = _fixture.CreateMany<BreweryWithBeers>().ToArray();
        _breweryBeersServiceMock.Setup(s => s.GetBreweryBeersAsync())
            .ReturnsAsync(breweriesWithBeers);
        
        var result = await GetDefaultSut().Get();

        result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(breweriesWithBeers);
    }
    
    [Test]
    public async Task Get_WhenNoBreweriesWithBeersExist_ReturnNoContent()
    {
        _breweryBeersServiceMock.Setup(s => s.GetBreweryBeersAsync())
            .ReturnsAsync(Array.Empty<BreweryWithBeers>());
        
        var result = await GetDefaultSut().Get();

        result.Should().BeOfType<NoContentResult>();
    }
    
    private BreweryBeersController GetDefaultSut() => new(_breweryBeersServiceMock.Object);
}