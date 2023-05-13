namespace Pss.ExampleBeers.Models.Model.Bars;

public record Bar(Guid Id, string Name, string Address)
{
    public static Bar Create(string name, string address)
    {
        return new Bar(Guid.NewGuid(), name, address);
    }
}