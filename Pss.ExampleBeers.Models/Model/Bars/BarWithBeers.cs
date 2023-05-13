using Pss.ExampleBeers.Models.Model.Beers;

namespace Pss.ExampleBeers.Models.Model.Bars;

public record BarWithBeers(Bar Bar, IReadOnlyList<Beer> Beers);