using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Pss.ExampleBeers.Models.Model.Breweries;

namespace Pss.ExampleBeers.Model.UnitTests.Breweries;

[TestFixture]
public class BreweryTests
{
    private readonly Fixture _fixture = new();

    [Test]
    public void Create_WhenCalled_CreatesNewBreweryWithId()
    {
        var inputs = new
        {
            Name = _fixture.Create<string>()
        };

        var result = Brewery.Create(inputs.Name);

        using (new AssertionScope())
        {
            result.Should().BeEquivalentTo(inputs);
            result.Id.Should().NotBeEmpty();
        }
    }
}