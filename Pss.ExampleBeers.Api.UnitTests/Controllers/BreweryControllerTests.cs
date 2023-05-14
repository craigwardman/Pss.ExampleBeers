using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Pss.ExampleBeers.Api.Controllers;
using Pss.ExampleBeers.Api.Models;
using Pss.ExampleBeers.ApplicationServices;
using Pss.ExampleBeers.Models.Model.Breweries;

namespace Pss.ExampleBeers.Api.UnitTests.Controllers;

[TestFixture]
public class BreweryControllerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IBreweryService> _breweryServiceMock = new();

    public BreweryControllerTests()
    {
        _fixture.Customize(new AutoMoqCustomization());
    }

    [SetUp]
    public void SetUp()
    {
        _breweryServiceMock.Reset();
    }

    [Test]
    public void Ctor_NullDependencies_Throws()
    {
        new GuardClauseAssertion(_fixture)
            .Verify(GetDefaultSut().GetType().GetConstructors());
    }
    
    [Test]
    public async Task Post_WhenCalled_CreatesBreweryAndReturnsCreated()
    {
        var request = _fixture.Create<BreweryRequestModel>();
        var createdBrewery = _fixture.Create<Brewery>();
        _breweryServiceMock.Setup(s => s.CreateAsync(request.Name))
            .ReturnsAsync(createdBrewery);
        
        var result = await GetDefaultSut().Post(request);

        result.Should().BeOfType<CreatedAtActionResult>()
            .Which.Value.Should().Be(createdBrewery.Id);
    }

    [Test]
    public async Task Get_WhenNoBreweriesExist_ReturnsNoContent()
    {
        _breweryServiceMock.Setup(s => s.GetAsync())
            .ReturnsAsync(Array.Empty<Brewery>());

        var result = await GetDefaultSut().Get();

        result.Should().BeOfType<NoContentResult>();
    }

    [Test]
    public async Task Get_WhenBreweriesExist_ReturnsOkObject()
    {
        var breweries = _fixture.CreateMany<Brewery>().ToArray();
        _breweryServiceMock.Setup(s => s.GetAsync()).ReturnsAsync(breweries);

        var result = await GetDefaultSut().Get();

        result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(breweries);
    }
    
    [Test]
    public async Task Get_WhenBreweryDoesNotExist_ReturnsNotFoundResult()
    {
        var breweryId = Guid.NewGuid();
        _breweryServiceMock.Setup(s => s.GetAsync(breweryId))
            .ReturnsAsync((Brewery?)null);

        var result = await GetDefaultSut().Get(breweryId);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Test]
    public async Task Get_WhenBreweryExists_ReturnsOkObject()
    {
        var breweryId = Guid.NewGuid();
        var brewery = _fixture.Create<Brewery>();
        _breweryServiceMock.Setup(s => s.GetAsync(breweryId)).ReturnsAsync(brewery);

        var result = await GetDefaultSut().Get(breweryId);

        result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(brewery);
    }
    
    [Test]
    public async Task Put_WhenUpdatedBreweryExists_ReturnsOkObject()
    {
        var request = _fixture.Create<BreweryRequestModel>();
        var breweryId = Guid.NewGuid();
        var updatedBrewery = _fixture.Create<Brewery>();
        _breweryServiceMock.Setup(s => s.UpdateAsync(breweryId, request.Name))
            .ReturnsAsync(updatedBrewery);
        
        var result = await GetDefaultSut().Put(breweryId, request);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(updatedBrewery);
    }
    
    [Test]
    public async Task Put_WhenUpdatedBreweryDoesNotExist_ReturnsNotFound()
    {
        var request = _fixture.Create<BreweryRequestModel>();
        var breweryId = Guid.NewGuid();
        _breweryServiceMock.Setup(s => s.UpdateAsync(breweryId, request.Name))
            .ReturnsAsync((Brewery?)null);
        
        var result = await GetDefaultSut().Put(breweryId, request);

        result.Should().BeOfType<NotFoundResult>();
    }
    
    private BreweryController GetDefaultSut() => new(_breweryServiceMock.Object);
}