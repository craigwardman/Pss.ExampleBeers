using Pss.ExampleBeers.Domain.Interfaces;
using Pss.ExampleBeers.Domain.Model.Beers;

namespace Pss.ExampleBeers.ApplicationServices;

public class BeerService : IBeerService
{
    private readonly IBeerRepository _beerRepository;

    public BeerService(IBeerRepository beerRepository)
    {
        _beerRepository = beerRepository ?? throw new ArgumentNullException(nameof(beerRepository));
    }

    public async Task<Beer> CreateAsync(string name, double percentageAlcoholByVolume)
    {
        var beer = Beer.Create(name, percentageAlcoholByVolume);
        await _beerRepository.UpsertBeerAsync(beer);

        return beer;
    }

    public Task<IReadOnlyList<Beer>> GetAsync(double? gtAlcoholByVolume, double? ltAlcoholByVolume)
    {
        return _beerRepository.GetBeersAsync(gtAlcoholByVolume, ltAlcoholByVolume);
    }

    public Task<Beer?> GetAsync(Guid id)
    {
        return _beerRepository.GetBeerAsync(id);
    }

    public async Task<Beer?> UpdateAsync(Guid id, string name, double percentageAlcoholByVolume)
    {
        var oldBeer = await GetAsync(id);
        if (oldBeer == null) return null;

        var newBeer = oldBeer with { Name = name, PercentageAlcoholByVolume = percentageAlcoholByVolume };
        await _beerRepository.UpsertBeerAsync(newBeer);

        return newBeer;
    }
}