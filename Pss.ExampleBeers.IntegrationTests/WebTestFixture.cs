using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Pss.ExampleBeers.IntegrationTests.Fakes;
using Pss.ExampleBeers.Models.Interfaces;
using Pss.ExampleBeers.Models.Model.Beers;
using Pss.ExampleBeers.MongoDB;
using Pss.ExampleBeers.MongoDB.Mongo;
using TestDataDefinitionFramework.Core;
using Program = Pss.ExampleBeers.Api.Program;

namespace Pss.ExampleBeers.IntegrationTests;

public class WebTestFixture : WebApplicationFactory<Program>
{
    private readonly FakeBeerRepository _fakeBeerRepository;
    private readonly FakeBreweryRepository _fakeBreweryRepository;
    private readonly FakeBarRepository _fakeBarRepository;
    private readonly FakeBreweryBeersRepository _fakeBreweryBeersRepository;
    private readonly FakeBarBeersRepository _fakeBarBeersRepository;

    public WebTestFixture(FakeBeerRepository fakeBeerRepository, FakeBreweryRepository fakeBreweryRepository, FakeBarRepository fakeBarRepository,
        FakeBreweryBeersRepository fakeBreweryBeersRepository, FakeBarBeersRepository fakeBarBeersRepository)
    {
        _fakeBeerRepository = fakeBeerRepository;
        _fakeBreweryRepository = fakeBreweryRepository;
        _fakeBarRepository = fakeBarRepository;
        _fakeBreweryBeersRepository = fakeBreweryBeersRepository;
        _fakeBarBeersRepository = fakeBarBeersRepository;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
#if !UseRealProvider
            services.AddTransient<IBeerRepository>(_ => _fakeBeerRepository.Mock.Object);
            services.AddTransient<IBreweryRepository>(_ => _fakeBreweryRepository.Mock.Object);
            services.AddTransient<IBarRepository>(_ => _fakeBarRepository.Mock.Object);
            services.AddTransient<IBreweryBeersRepository>(_ => _fakeBreweryBeersRepository.Mock.Object);
            services.AddTransient<IBarBeersRepository>(_ => _fakeBarBeersRepository.Mock.Object);
#else
            services.Configure<MongoDataStoreConfig>(cfg =>
            {
                cfg.ConnectionString = TestDataStore.Repository<Beer>(BeersCollection.Name)
                    .Config.BackingStore.ConnectionString;
            });
#endif
        });
    }
}