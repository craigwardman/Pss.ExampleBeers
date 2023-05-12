using Pss.ExampleBeers.Domain.Model.Beers;

namespace Pss.ExampleBeers.Domain.Interfaces;

public interface IBeerRepository
{
    Task<IReadOnlyList<Beer>> GetBeersAsync(double? gtAlcoholByVolume, double? ltAlcoholByValue);
    Task UpsertBeerAsync(Beer beer);
    Task<Beer?> GetBeerAsync(Guid id);
}