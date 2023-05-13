using Pss.ExampleBeers.Models.Model.Bars;

namespace Pss.ExampleBeers.Models.Interfaces;

public interface IBarBeersRepository
{
    Task InsertAsync(BarBeer breweryBeer);

    Task<BarWithBeers?> GetBarBeersAsync(Guid breweryId);

    Task<IReadOnlyList<BarWithBeers>> GetBarBeersAsync();
}