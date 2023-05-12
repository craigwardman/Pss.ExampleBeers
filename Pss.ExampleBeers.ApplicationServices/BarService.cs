using Pss.ExampleBeers.Domain.Interfaces;
using Pss.ExampleBeers.Domain.Model.Bars;
using Pss.ExampleBeers.Domain.Model.Breweries;

namespace Pss.ExampleBeers.ApplicationServices;

public class BarService : IBarService
{
    private readonly IBarRepository _barRepository;

    public BarService(IBarRepository barRepository)
    {
        _barRepository = barRepository ?? throw new ArgumentNullException(nameof(barRepository));
    }

    public async Task<Bar> CreateAsync(string name, string address)
    {
        var beer = Bar.Create(name, address);
        await _barRepository.UpsertBarAsync(beer);

        return beer;
    }

    public Task<IReadOnlyList<Bar>> GetAsync()
    {
        return _barRepository.GetBarsAsync();
    }

    public Task<Bar?> GetAsync(Guid id)
    {
        return _barRepository.GetBarAsync(id);
    }

    public async Task<Bar?> UpdateAsync(Guid id, string name, string address)
    {
        var oldBar = await GetAsync(id);
        if (oldBar == null) return null;

        var newBar = oldBar with { Name = name, Address = address };
        await _barRepository.UpsertBarAsync(newBar);

        return newBar;
    }
}