using Pss.ExampleBeers.Domain.Model.Beers;

namespace Pss.ExampleBeers.Domain.Model.Breweries;

public record Brewery(Guid Id, string Name, IReadOnlyList<Beer> BrewedBeers)
{
    private readonly List<Beer> _brewedBeers = BrewedBeers.ToList();

    public void AddBeer(Beer beer)
    {
        _brewedBeers.Add(beer);
    }

    public IReadOnlyList<Beer> BrewedBeers => _brewedBeers;
}