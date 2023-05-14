using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using Pss.ExampleBeers.Models.Interfaces;
using Pss.ExampleBeers.Models.Model.Bars;

namespace Pss.ExampleBeers.ApplicationServices.UnitTests;

[TestFixture]
public class BarServiceTests
{
    [SetUp]
    public void SetUp()
    {
        _barRepositoryMock.Reset();
    }

    private readonly Fixture _fixture = new();
    private readonly Mock<IBarRepository> _barRepositoryMock = new();

    public BarServiceTests()
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
    public async Task CreateAsync_WhenCalled_InsertsNewBar()
    {
        var inputs = new
        {
            Name = _fixture.Create<string>(),
            Address = _fixture.Create<string>()
        };

        var result = await GetDefaultSut().CreateAsync(inputs.Name, inputs.Address);

        using (new AssertionScope())
        {
            result.Should().BeEquivalentTo(inputs);
            _barRepositoryMock.Verify(r => r.UpsertBarAsync(result));
        }
    }
    
    [Test]
    public async Task GetAsync_WhenCalled_GetsRecordFromRepository()
    {
        var record = _fixture.Create<Bar>();
        _barRepositoryMock.Setup(r => r.GetBarAsync(record.Id)).ReturnsAsync(record);

        var result = await GetDefaultSut().GetAsync(record.Id);
        
        result.Should().Be(record);
    }
    
    [Test]
    public async Task GetAsync_WhenCalled_GetsRecordsFromRepository()
    {
        var records = _fixture.CreateMany<Bar>().ToArray();
        _barRepositoryMock.Setup(r => r.GetBarsAsync()).ReturnsAsync(records);

        var results = await GetDefaultSut().GetAsync();
        
        results.Should().BeEquivalentTo(records);
    }
    
    [Test]
    public async Task UpdateAsync_WhenCalledForExistingBar_UpdatesBar()
    {
        var existingBar = _fixture.Create<Bar>();
        var inputs = new
        {
            existingBar.Id,
            Name = _fixture.Create<string>(),
            Address = _fixture.Create<string>()
        };
        _barRepositoryMock.Setup(r => r.GetBarAsync(existingBar.Id)).ReturnsAsync(existingBar);

        var result = await GetDefaultSut().UpdateAsync(inputs.Id, inputs.Name, inputs.Address);

        using (new AssertionScope())
        {
            result.Should().BeEquivalentTo(inputs);
            _barRepositoryMock.Verify(r => r.UpsertBarAsync(result!));
        }
    }

    [Test] public async Task UpdateAsync_WhenCalledForNonExistentBar_ReturnsNull()
    {
        var inputs = new
        {
            Id = Guid.NewGuid(),
            Name = _fixture.Create<string>(),
            Address = _fixture.Create<string>()
        };
        _barRepositoryMock.Setup(r => r.GetBarAsync(It.IsAny<Guid>())).ReturnsAsync((Bar?)null);

        var result = await GetDefaultSut().UpdateAsync(inputs.Id, inputs.Name, inputs.Address);

        using (new AssertionScope())
        {
            result.Should().BeNull();
            _barRepositoryMock.Verify(r => r.UpsertBarAsync(It.IsAny<Bar>()), Times.Never);
        }
    }

    private BarService GetDefaultSut() => new(_barRepositoryMock.Object);
}