using Pss.ExampleBeers.Models.Model.Breweries;

namespace Pss.ExampleBeers.Models.Interfaces;

public interface IBreweryRepository
{
    Task<IReadOnlyList<Brewery>> GetBreweriesAsync();
    Task UpsertBreweryAsync(Brewery brewery);
    Task<Brewery?> GetBreweryAsync(Guid id);
}