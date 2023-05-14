using Pss.ExampleBeers.Models.Model.Bars;
using Pss.ExampleBeers.Models.Model.Beers;

namespace Pss.ExampleBeers.ApplicationServices;

public interface IBarBeersService
{
    Task<(Bar? bar, Beer? beer)> LinkBeerAsync(Guid barId, Guid beerId);
    Task<BarWithBeers?> GetBarBeersAsync(Guid barId);
    Task<IReadOnlyList<BarWithBeers>> GetBarBeersAsync();
}