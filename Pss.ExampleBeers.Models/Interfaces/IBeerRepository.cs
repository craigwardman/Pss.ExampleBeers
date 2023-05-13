using Pss.ExampleBeers.Models.Model.Beers;

namespace Pss.ExampleBeers.Models.Interfaces;

public interface IBeerRepository
{
    Task<IReadOnlyList<Beer>> GetBeersAsync(double? gtAlcoholByVolume, double? ltAlcoholByValue);
    Task UpsertBeerAsync(Beer beer);
    Task<Beer?> GetBeerAsync(Guid id);
}