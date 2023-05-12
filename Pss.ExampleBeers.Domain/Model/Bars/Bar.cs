using Pss.ExampleBeers.Domain.Model.Beers;

namespace Pss.ExampleBeers.Domain.Model.Bars;

public record Bar(Guid Id, string Name, string Address, IReadOnlyList<Beer> ServedBeers)
{
    private readonly List<Beer> _servedBeers = ServedBeers.ToList();

    public void AddBeer(Beer beer)
    {
        _servedBeers.Add(beer);
    }

    public IReadOnlyList<Beer> ServedBeers => _servedBeers;
}