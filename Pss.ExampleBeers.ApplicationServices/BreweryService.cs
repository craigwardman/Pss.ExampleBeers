using Pss.ExampleBeers.Models.Interfaces;
using Pss.ExampleBeers.Models.Model.Breweries;

namespace Pss.ExampleBeers.ApplicationServices;

public class BreweryService : IBreweryService
{
    private readonly IBreweryRepository _breweryRepository;
    
    public BreweryService(IBreweryRepository breweryRepository)
    {
        _breweryRepository = breweryRepository ?? throw new ArgumentNullException(nameof(breweryRepository));
    }

    public async Task<Brewery> CreateAsync(string name)
    {
        var beer = Brewery.Create(name);
        await _breweryRepository.UpsertBreweryAsync(beer);

        return beer;
    }

    public Task<IReadOnlyList<Brewery>> GetAsync()
    {
        return _breweryRepository.GetBreweriesAsync();
    }

    public Task<Brewery?> GetAsync(Guid id)
    {
        return _breweryRepository.GetBreweryAsync(id);
    }

    public async Task<Brewery?> UpdateAsync(Guid id, string name)
    {
        var oldBrewery = await GetAsync(id);
        if (oldBrewery == null) return null;

        var newBrewery = oldBrewery with { Name = name };
        await _breweryRepository.UpsertBreweryAsync(newBrewery);

        return newBrewery;
    }
}