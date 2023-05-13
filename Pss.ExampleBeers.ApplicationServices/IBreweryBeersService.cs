using Pss.ExampleBeers.Models.Model.Beers;
using Pss.ExampleBeers.Models.Model.Breweries;

namespace Pss.ExampleBeers.ApplicationServices;

public interface IBreweryBeersService
{
    Task<(Brewery? brewery, Beer? beer)> LinkBeerAsync(Guid breweryId, Guid beerId);
    Task<BreweryWithBeers?> GetBreweryBeersAsync(Guid breweryId);
    Task<IReadOnlyList<BreweryWithBeers>> GetBreweryBeersAsync();
}