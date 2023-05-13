using Pss.ExampleBeers.Models.Model.Bars;
using Pss.ExampleBeers.Models.Model.Beers;
using Pss.ExampleBeers.Models.Model.Breweries;
using Pss.ExampleBeers.MongoDB.Mongo;
using TechTalk.SpecFlow;
using TestDataDefinitionFramework.Core;
using TestDataDefinitionFramework.MongoDB;

namespace Pss.ExampleBeers.IntegrationTests;

[Binding]
public class TestDataSourceHooks
{
    private readonly ScenarioContext _scenarioContext;

    public TestDataSourceHooks(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [BeforeTestRun]
    public static async Task Initialize()
    {
#if UseRealProvider
        var mongoBackingStore = new MongoBackingStore("example_beers");
#endif

        TestDataStore.AddRepository<Beer>(cfg =>
        {
            cfg.WithName(BeersCollection.Name);
#if UseRealProvider
            cfg.WithBackingStore(mongoBackingStore);
#endif
        });
        
        TestDataStore.AddRepository<Brewery>(cfg =>
        {
            cfg.WithName(BreweriesCollection.Name);
#if UseRealProvider
            cfg.WithBackingStore(mongoBackingStore);
#endif
        });
        
        TestDataStore.AddRepository<BreweryBeer>(cfg =>
        {
            cfg.WithName(BreweryBeersCollection.Name);
#if UseRealProvider
            cfg.WithBackingStore(mongoBackingStore);
#endif
        });
        
        TestDataStore.AddRepository<Bar>(cfg =>
        {
            cfg.WithName(BarsCollection.Name);
#if UseRealProvider
            cfg.WithBackingStore(mongoBackingStore);
#endif
        });
        
        await TestDataStore.InitializeAllAsync();
    }

    [BeforeScenarioBlock]
    public async Task Commit()
    {
        if (_scenarioContext.CurrentScenarioBlock == ScenarioBlock.When) await TestDataStore.CommitAllAsync();
    }
}