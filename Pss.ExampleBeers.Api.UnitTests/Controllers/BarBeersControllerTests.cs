using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Pss.ExampleBeers.Api.Controllers;
using Pss.ExampleBeers.Api.Models;
using Pss.ExampleBeers.ApplicationServices;
using Pss.ExampleBeers.Models.Model.Bars;
using Pss.ExampleBeers.Models.Model.Beers;

namespace Pss.ExampleBeers.Api.UnitTests.Controllers;

[TestFixture]
public class BarBeersControllerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IBarBeersService> _barBeersServiceMock = new();

    public BarBeersControllerTests()
    {
        _fixture.Customize(new AutoMoqCustomization());
    }

    [SetUp]
    public void SetUp()
    {
        _barBeersServiceMock.Reset();
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
        var request = _fixture.Create<BarBeerLinkModel>();

        await GetDefaultSut().Post(request);
        
        _barBeersServiceMock.Verify(s => s.LinkBeerAsync(request.BarId, request.BeerId));
    }
    
    [Test]
    public async Task Post_WhenCalledWithNonExistentBeerId_ReturnsBadRequest()
    {
        _barBeersServiceMock.Setup(s => s.LinkBeerAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync((_fixture.Create<Bar>(), null));

        var result = await GetDefaultSut().Post(_fixture.Create<BarBeerLinkModel>());

        result.Should().BeOfType<BadRequestObjectResult>();
    }
    
    [Test]
    public async Task Post_WhenCalledWithNonExistentBarId_ReturnsBadRequest()
    {
        _barBeersServiceMock.Setup(s => s.LinkBeerAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync((null, _fixture.Create<Beer>()));

        var result = await GetDefaultSut().Post(_fixture.Create<BarBeerLinkModel>());

        result.Should().BeOfType<BadRequestObjectResult>();
    }
    
    [Test]
    public async Task Post_WhenLinkSuccessful_ReturnsNoContent()
    {
        _barBeersServiceMock.Setup(s => s.LinkBeerAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync((_fixture.Create<Bar>(), _fixture.Create<Beer>()));

        var result = await GetDefaultSut().Post(_fixture.Create<BarBeerLinkModel>());

        result.Should().BeOfType<NoContentResult>();
    }

    [Test]
    public async Task Get_WhenBarWithBeersExists_ReturnOkObject()
    {
        var barId = Guid.NewGuid();
        var barWithBeers = _fixture.Create<BarWithBeers>();
        _barBeersServiceMock.Setup(s => s.GetBarBeersAsync(barId))
            .ReturnsAsync(barWithBeers);
        
        var result = await GetDefaultSut().Get(barId);

        result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(barWithBeers);
    }
    
    [Test]
    public async Task Get_WhenBarWithBeersDoesNotExist_ReturnNotFound()
    {
        var barId = Guid.NewGuid();
        _barBeersServiceMock.Setup(s => s.GetBarBeersAsync(barId))
            .ReturnsAsync((BarWithBeers?)null);
        
        var result = await GetDefaultSut().Get(barId);

        result.Should().BeOfType<NotFoundResult>();
    }
    
    [Test]
    public async Task Get_WhenBarsWithBeersExists_ReturnOkObject()
    {
        var barsWithBeers = _fixture.CreateMany<BarWithBeers>().ToArray();
        _barBeersServiceMock.Setup(s => s.GetBarBeersAsync())
            .ReturnsAsync(barsWithBeers);
        
        var result = await GetDefaultSut().Get();

        result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(barsWithBeers);
    }
    
    [Test]
    public async Task Get_WhenNoBarsWithBeersExist_ReturnNoContent()
    {
        _barBeersServiceMock.Setup(s => s.GetBarBeersAsync())
            .ReturnsAsync(Array.Empty<BarWithBeers>());
        
        var result = await GetDefaultSut().Get();

        result.Should().BeOfType<NoContentResult>();
    }
    
    private BarBeersController GetDefaultSut() => new(_barBeersServiceMock.Object);
}