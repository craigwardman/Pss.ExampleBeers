using Pss.ExampleBeers.Models.Model.Bars;

namespace Pss.ExampleBeers.Models.Interfaces;

public interface IBarRepository
{
    Task<IReadOnlyList<Bar>> GetBarsAsync();
    Task UpsertBarAsync(Bar brewery);
    Task<Bar?> GetBarAsync(Guid id);
}