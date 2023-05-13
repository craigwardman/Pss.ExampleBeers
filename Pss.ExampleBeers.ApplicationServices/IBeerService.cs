using Pss.ExampleBeers.Models.Model.Beers;

namespace Pss.ExampleBeers.ApplicationServices;

public interface IBeerService
{
    Task<Beer> CreateAsync(string name, double percentageAlcoholByVolume);
    Task<IReadOnlyList<Beer>> GetAsync(double? gtAlcoholByVolume, double? ltAlcoholByVolume);
    Task<Beer?> GetAsync(Guid id);
    Task<Beer?> UpdateAsync(Guid id, string name, double percentageAlcoholByVolume);
}