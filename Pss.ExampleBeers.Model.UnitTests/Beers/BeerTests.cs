using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Pss.ExampleBeers.Models.Model.Beers;

namespace Pss.ExampleBeers.Model.UnitTests.Beers;

[TestFixture]
public class BeerTests
{
    private readonly Fixture _fixture = new();

    [Test]
    public void Create_WhenCalled_CreatesNewBeerWithId()
    {
        var inputs = new
        {
            Name = _fixture.Create<string>(),
            PercentageAlcoholByVolume = _fixture.Create<double>()
        };

        var result = Beer.Create(inputs.Name, inputs.PercentageAlcoholByVolume);

        using (new AssertionScope())
        {
            result.Should().BeEquivalentTo(inputs);
            result.Id.Should().NotBeEmpty();
        }
    }
}