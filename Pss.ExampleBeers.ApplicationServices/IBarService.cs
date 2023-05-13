using Pss.ExampleBeers.Models.Model.Bars;

namespace Pss.ExampleBeers.ApplicationServices;

public interface IBarService
{
    Task<Bar> CreateAsync(string name, string address);
    Task<IReadOnlyList<Bar>> GetAsync();
    Task<Bar?> GetAsync(Guid id);
    Task<Bar?> UpdateAsync(Guid id, string name, string address);
}