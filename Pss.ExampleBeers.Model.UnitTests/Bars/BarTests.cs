using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Pss.ExampleBeers.Models.Model.Bars;

namespace Pss.ExampleBeers.Model.UnitTests.Bars;

[TestFixture]
public class BarTests
{
    private readonly Fixture _fixture = new();

    [Test]
    public void Create_WhenCalled_CreatesNewBarWithId()
    {
        var inputs = new
        {
            Name = _fixture.Create<string>(),
            Address = _fixture.Create<string>()
        };

        var result = Bar.Create(inputs.Name, inputs.Address);

        using (new AssertionScope())
        {
            result.Should().BeEquivalentTo(inputs);
            result.Id.Should().NotBeEmpty();
        }
    }
}