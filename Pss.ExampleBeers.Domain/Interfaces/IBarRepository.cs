using Pss.ExampleBeers.Domain.Model.Bars;

namespace Pss.ExampleBeers.Domain.Interfaces;

public interface IBarRepository
{
    Task<IReadOnlyList<Bar>> GetBarsAsync();
    Task UpsertBarAsync(Bar brewery);
    Task<Bar?> GetBarAsync(Guid id);
}