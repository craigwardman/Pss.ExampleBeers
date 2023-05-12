using Pss.ExampleBeers.Domain.Model.Breweries;

namespace Pss.ExampleBeers.ApplicationServices;

public interface IBreweryService
{
    Task<Brewery> CreateAsync(string name);
    Task<IReadOnlyList<Brewery>> GetAsync();
    Task<Brewery?> GetAsync(Guid id);
    Task<Brewery?> UpdateAsync(Guid id, string name);
}