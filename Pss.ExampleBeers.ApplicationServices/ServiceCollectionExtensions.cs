using Microsoft.Extensions.DependencyInjection;

namespace Pss.ExampleBeers.ApplicationServices;

public static class ServiceCollectionExtensions
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IBeerService, BeerService>();
        
        services.AddTransient<IBreweryService, BreweryService>();
        services.AddTransient<IBreweryBeersService, BreweryBeersService>();
        
        services.AddTransient<IBarService, BarService>();
        services.AddTransient<IBarBeersService, BarBeersService>();
    }
}