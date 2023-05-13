using Pss.ExampleBeers.Models.Model;
using Pss.ExampleBeers.Models.Model.Breweries;

namespace Pss.ExampleBeers.Models.Interfaces;

public interface IBreweryBeersRepository
{
    Task InsertAsync(BreweryBeer breweryBeer);
    
    Task<BreweryWithBeers?> GetBreweryBeersAsync(Guid breweryId);
    
    Task<IReadOnlyList<BreweryWithBeers>> GetBreweryBeersAsync();
}