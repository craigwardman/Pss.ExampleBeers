using Pss.ExampleBeers.Models.Interfaces;
using Pss.ExampleBeers.Models.Model.Bars;
using Pss.ExampleBeers.Models.Model.Beers;

namespace Pss.ExampleBeers.ApplicationServices;

public class BarBeersService : IBarBeersService
{
    private readonly IBarBeersRepository _barBeerRepository;
    private readonly IBarService _barService;
    private readonly IBeerService _beerService;

    public BarBeersService(IBarService barService, IBeerService beerService, IBarBeersRepository barBeerRepository)
    {
        _barService = barService ?? throw new ArgumentNullException(nameof(barService));
        _beerService = beerService ?? throw new ArgumentNullException(nameof(beerService));
        _barBeerRepository = barBeerRepository ?? throw new ArgumentNullException(nameof(barBeerRepository));
    }

    public async Task<(Bar? bar, Beer? beer)> LinkBeerAsync(Guid barId, Guid beerId)
    {
        var bar = await _barService.GetAsync(barId);
        var beer = await _beerService.GetAsync(beerId);

        if (bar != null && beer != null)
        {
            var barBeer = new BarBeer(bar.Id, beer.Id);
            await _barBeerRepository.InsertAsync(barBeer);
        }

        return (bar, beer);
    }

    public Task<BarWithBeers?> GetBarBeersAsync(Guid barId)
    {
        return _barBeerRepository.GetBarBeersAsync(barId);
    }

    public Task<IReadOnlyList<BarWithBeers>> GetBarBeersAsync()
    {
        return _barBeerRepository.GetBarBeersAsync();
    }
}