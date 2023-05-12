using Pss.ExampleBeers.Api.Models;
using Pss.ExampleBeers.Domain.Model.Beers;
using Pss.ExampleBeers.Domain.Model.Breweries;
using Pss.ExampleBeers.MongoDB.Mongo;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using TestDataDefinitionFramework.Core;

namespace Pss.ExampleBeers.IntegrationTests.Fakes;

[Binding]
public class Steps
{
    [Given(@"the database contains a beer called '(.*)' that is '(.*)' by volume")]
    public void GivenTheDatabaseContainsABeerCalledThatIsByVolume(string name, double percentageAlcoholByVolume)
    {
        TestDataStore.Repository<Beer>(BeersCollection.Name).Items = new[]
        {
            Beer.Create(name, percentageAlcoholByVolume)
        };
    }

    [Given(@"the database contains several beers, as per below")]
    public void GivenTheDatabaseContainsSeveralBeersAsPerBelow(Table table)
    {
        TestDataStore.Repository<Beer>(BeersCollection.Name).Items = table.CreateSet<BeerRequestModel>()
            .Select(r => Beer.Create(r.Name, r.PercentageAlcoholByVolume))
            .ToArray();
    }
    
    [Given(@"the database contains a brewery called '(.*)'")]
    public void GivenTheDatabaseContainsABreweryCalled(string name)
    {
        TestDataStore.Repository<Brewery>(BreweriesCollection.Name).Items = new[]
        {
            Brewery.Create(name)
        };
    }

    [Given(@"the database contains several breweries, as per below")]
    public void GivenTheDatabaseContainsSeveralBreweriesAsPerBelow(Table table)
    {
        TestDataStore.Repository<Brewery>(BreweriesCollection.Name).Items = table.CreateSet<BreweryRequestModel>()
            .Select(r => Brewery.Create(r.Name))
            .ToArray();
    }
}