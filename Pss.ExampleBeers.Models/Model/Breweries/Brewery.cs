namespace Pss.ExampleBeers.Models.Model.Breweries;

public record Brewery(Guid Id, string Name)
{
    public static Brewery Create(string name)
    {
        return new Brewery(Guid.NewGuid(), name);
    }
}