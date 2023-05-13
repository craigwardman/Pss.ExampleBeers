namespace Pss.ExampleBeers.Models.Model.Beers;

public record Beer(Guid Id, string Name, double PercentageAlcoholByVolume)
{
    public static Beer Create(string name, double percentageAlcoholByVolume)
    {
        return new Beer(Guid.NewGuid(), name, percentageAlcoholByVolume);
    }
}