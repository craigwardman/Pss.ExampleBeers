using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using Pss.ExampleBeers.Models.Interfaces;
using Pss.ExampleBeers.Models.Model.Breweries;
using Pss.ExampleBeers.Models.Model.Breweries;

namespace Pss.ExampleBeers.ApplicationServices.UnitTests;

[TestFixture]
public class BreweryServiceTests
{
    [SetUp]
    public void SetUp()
    {
        _breweryRepositoryMock.Reset();
    }

    private readonly Fixture _fixture = new();
    private readonly Mock<IBreweryRepository> _breweryRepositoryMock = new();

    public BreweryServiceTests()
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
    public async Task CreateAsync_WhenCalled_InsertsNewBrewery()
    {
        var name = _fixture.Create<string>();
        var result = await GetDefaultSut().CreateAsync(name);

        using (new AssertionScope())
        {
            result.Name.Should().Be(name);
            _breweryRepositoryMock.Verify(r => r.UpsertBreweryAsync(result));
        }
    }

    [Test]
    public async Task GetAsync_WhenCalled_GetsRecordFromRepository()
    {
        var record = _fixture.Create<Brewery>();
        _breweryRepositoryMock.Setup(r => r.GetBreweryAsync(record.Id)).ReturnsAsync(record);

        var result = await GetDefaultSut().GetAsync(record.Id);

        result.Should().Be(record);
    }

    [Test]
    public async Task GetAsync_WhenCalled_GetsRecordsFromRepository()
    {
        var records = _fixture.CreateMany<Brewery>().ToArray();
        _breweryRepositoryMock.Setup(r => r.GetBreweriesAsync()).ReturnsAsync(records);

        var results = await GetDefaultSut().GetAsync();

        results.Should().BeEquivalentTo(records);
    }

    [Test]
    public async Task UpdateAsync_WhenCalledForExistingBrewery_UpdatesBrewery()
    {
        var existingBrewery = _fixture.Create<Brewery>();
        var inputs = new
        {
            existingBrewery.Id,
            Name = _fixture.Create<string>(),
        };
        _breweryRepositoryMock.Setup(r => r.GetBreweryAsync(existingBrewery.Id)).ReturnsAsync(existingBrewery);

        var result = await GetDefaultSut().UpdateAsync(inputs.Id, inputs.Name);

        using (new AssertionScope())
        {
            result.Should().BeEquivalentTo(inputs);
            _breweryRepositoryMock.Verify(r => r.UpsertBreweryAsync(result!));
        }
    }

    [Test]
    public async Task UpdateAsync_WhenCalledForNonExistentBrewery_ReturnsNull()
    {
        var inputs = new
        {
            Id = Guid.NewGuid(),
            Name = _fixture.Create<string>()
        };
        _breweryRepositoryMock.Setup(r => r.GetBreweryAsync(It.IsAny<Guid>())).ReturnsAsync((Brewery?)null);

        var result = await GetDefaultSut().UpdateAsync(inputs.Id, inputs.Name);

        using (new AssertionScope())
        {
            result.Should().BeNull();
            _breweryRepositoryMock.Verify(r => r.UpsertBreweryAsync(It.IsAny<Brewery>()), Times.Never);
        }
    }

    private BreweryService GetDefaultSut() => new(_breweryRepositoryMock.Object);
}