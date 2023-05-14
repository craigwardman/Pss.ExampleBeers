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

namespace Pss.ExampleBeers.Api.UnitTests.Controllers;

[TestFixture]
public class BarControllerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IBarService> _barServiceMock = new();

    public BarControllerTests()
    {
        _fixture.Customize(new AutoMoqCustomization());
    }

    [SetUp]
    public void SetUp()
    {
        _barServiceMock.Reset();
    }

    [Test]
    public void Ctor_NullDependencies_Throws()
    {
        new GuardClauseAssertion(_fixture)
            .Verify(GetDefaultSut().GetType().GetConstructors());
    }

    [Test]
    public async Task Post_WhenCalled_CreatesBarAndReturnsCreated()
    {
        var request = _fixture.Create<BarRequestModel>();
        var createdBar = _fixture.Create<Bar>();
        _barServiceMock.Setup(s => s.CreateAsync(request.Name, request.Address))
            .ReturnsAsync(createdBar);
        
        var result = await GetDefaultSut().Post(request);

        result.Should().BeOfType<CreatedAtActionResult>()
            .Which.Value.Should().Be(createdBar.Id);
    }

    [Test]
    public async Task Get_WhenNoBarsExist_ReturnsNoContent()
    {
        _barServiceMock.Setup(s => s.GetAsync())
            .ReturnsAsync(Array.Empty<Bar>());

        var result = await GetDefaultSut().Get();

        result.Should().BeOfType<NoContentResult>();
    }

    [Test]
    public async Task Get_WhenBarsExist_ReturnsOkObject()
    {
        var bars = _fixture.CreateMany<Bar>().ToArray();
        _barServiceMock.Setup(s => s.GetAsync()).ReturnsAsync(bars);

        var result = await GetDefaultSut().Get();

        result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(bars);
    }
    
    [Test]
    public async Task Get_WhenBarDoesNotExist_ReturnsNotFoundResult()
    {
        var barId = Guid.NewGuid();
        _barServiceMock.Setup(s => s.GetAsync(barId))
            .ReturnsAsync((Bar?)null);

        var result = await GetDefaultSut().Get(barId);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Test]
    public async Task Get_WhenBarExists_ReturnsOkObject()
    {
        var barId = Guid.NewGuid();
        var bar = _fixture.Create<Bar>();
        _barServiceMock.Setup(s => s.GetAsync(barId)).ReturnsAsync(bar);

        var result = await GetDefaultSut().Get(barId);

        result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(bar);
    }
    
    [Test]
    public async Task Put_WhenUpdatedBarExists_ReturnsOkObject()
    {
        var request = _fixture.Create<BarRequestModel>();
        var barId = Guid.NewGuid();
        var updatedBar = _fixture.Create<Bar>();
        _barServiceMock.Setup(s => s.UpdateAsync(barId, request.Name, request.Address))
            .ReturnsAsync(updatedBar);
        
        var result = await GetDefaultSut().Put(barId, request);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(updatedBar);
    }
    
    [Test]
    public async Task Put_WhenUpdatedBarDoesNotExist_ReturnsNotFound()
    {
        var request = _fixture.Create<BarRequestModel>();
        var barId = Guid.NewGuid();
        _barServiceMock.Setup(s => s.UpdateAsync(barId, request.Name, request.Address))
            .ReturnsAsync((Bar?)null);
        
        var result = await GetDefaultSut().Put(barId, request);

        result.Should().BeOfType<NotFoundResult>();
    }
    
    private BarController GetDefaultSut() => new(_barServiceMock.Object);
}