using Pss.ExampleBeers.Domain.Model.Beers;
using Pss.ExampleBeers.Domain.Model.Breweries;

namespace Pss.ExampleBeers.Domain.Interfaces;

public interface IBreweryRepository
{
    Task<IReadOnlyList<Brewery>> GetBreweriesAsync();
    Task UpsertBreweryAsync(Brewery brewery);
    Task<Brewery?> GetBreweryAsync(Guid id);
}