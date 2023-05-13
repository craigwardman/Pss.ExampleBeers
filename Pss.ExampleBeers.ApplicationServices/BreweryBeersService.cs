using Pss.ExampleBeers.Models.Interfaces;
using Pss.ExampleBeers.Models.Model.Beers;
using Pss.ExampleBeers.Models.Model.Breweries;

namespace Pss.ExampleBeers.ApplicationServices;

public class BreweryBeersService : IBreweryBeersService
{
    private readonly IBreweryService _breweryService;
    private readonly IBeerService _beerService;
    private readonly IBreweryBeersRepository _breweryBeerRepository;

    public BreweryBeersService(IBreweryService breweryService, IBeerService beerService, IBreweryBeersRepository breweryBeerRepository)
    {
        _breweryService = breweryService ?? throw new ArgumentNullException(nameof(breweryService));
        _beerService = beerService ?? throw new ArgumentNullException(nameof(beerService));
        _breweryBeerRepository = breweryBeerRepository ?? throw new ArgumentNullException(nameof(breweryBeerRepository));
    }

    public async Task<(Brewery? brewery, Beer? beer)> LinkBeerAsync(Guid breweryId, Guid beerId)
    {
        var brewery = await _breweryService.GetAsync(breweryId);
        var beer = await _beerService.GetAsync(beerId);

        if (brewery != null && beer != null)
        {
            var breweryBeer = new BreweryBeer(brewery.Id, beer.Id);
            await _breweryBeerRepository.InsertAsync(breweryBeer);
        }

        return (brewery, beer);
    }

    public Task<BreweryWithBeers?> GetBreweryBeersAsync(Guid breweryId)
    {
        return _breweryBeerRepository.GetBreweryBeersAsync(breweryId);
    }

    public Task<IReadOnlyList<BreweryWithBeers>> GetBreweryBeersAsync()
    {
        return _breweryBeerRepository.GetBreweryBeersAsync();
    }
}