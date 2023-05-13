using Pss.ExampleBeers.Models.Model.Beers;

namespace Pss.ExampleBeers.Models.Model.Breweries;

public record BreweryWithBeers(Brewery Brewery, IReadOnlyList<Beer> Beers);