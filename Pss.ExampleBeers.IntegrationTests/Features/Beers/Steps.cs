using System.Diagnostics;
using FluentAssertions;
using FluentAssertions.Execution;
using Pss.ExampleBeers.Api.Models;
using Pss.ExampleBeers.Models.Model.Beers;
using Pss.ExampleBeers.MongoDB.Mongo;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using TestDataDefinitionFramework.Core;

namespace Pss.ExampleBeers.IntegrationTests.Features.Beers;

[Binding]
public class Steps
{
    private readonly ApiClient _apiClient;
    private Beer? _retrievedBeer;
    private IReadOnlyList<Beer> _retrievedBeers = Array.Empty<Beer>();

    public Steps(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    [When(@"I send a request to create a beer called '(.*)' that is '(.*)' by volume")]
    public async Task WhenISendARequestToCreateABeerCalledThatIsByVolume(string name, double percentageAlcoholByVolume)
    {
        await _apiClient.PostAsync(name, percentageAlcoholByVolume);
    }

    [Then(@"I get a successful beers response")]
    public void ThenIGetASuccessfulBeersResponse()
    {
        _apiClient.Response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Then(@"my new beer is given an ID")]
    public void ThenMyNewBeerIsGivenAnId()
    {
        _apiClient.Response.Content.ReadAsAsync<Guid>().Should().NotBeSameAs(Guid.Empty);
    }

    [When(@"I request the '(.*)' beer by it's ID")]
    public async Task WhenIRequestTheBeerByItsId(string name)
    {
        var id = TestDataStore.Repository<Beer>(BeersCollection.Name).Items.Single(i => i.Name == name).Id;
        _retrievedBeer = await _apiClient.GetAsync(id);
    }

    [Then(@"the returned beer is called '(.*)' that is '(.*)' by volume")]
    public void ThenTheReturnedBeerIsCalledThatIsByVolume(string name, double percentageAlcoholByVolume)
    {
        using (new AssertionScope())
        {
            Debug.Assert(_retrievedBeer != null);
            
            _retrievedBeer.Name.Should().Be(name);
            _retrievedBeer.PercentageAlcoholByVolume.Should().Be(percentageAlcoholByVolume);
        }
    }

    [When(@"I request to update the '(.*)' beer to be called '(.*)' that is '(.*)' by volume")]
    public async Task WhenIRequestToUpdateTheBeerToBeCalledThatIsByVolume(string name, string newName, double newPercentageAlcoholByVolume)
    {
        var id = TestDataStore.Repository<Beer>(BeersCollection.Name).Items.Single(i => i.Name == name).Id;
        _retrievedBeer = await _apiClient.PutAsync(id, newName, newPercentageAlcoholByVolume);
    }

    [When(@"I request all the beers")]
    public async Task WhenIRequestAllTheBeers()
    {
        _retrievedBeers = await _apiClient.GetAsync();
    }

    [Then(@"the returned list of beers is as below")]
    public void ThenTheReturnedListOfBeersIsAsBelow(Table table)
    {
        var expected = table.CreateSet<BeerRequestModel>().ToList();

        _retrievedBeers.Should().BeEquivalentTo(expected);
    }

    [When(@"I request the beers between greater than '(.*)' and less than '(.*)' volume")]
    public async Task WhenIRequestTheBeersBetweenGreaterThanAndLessThanVolume(double gtAlcoholByVolume, double ltAlcoholByVolume)
    {
        _retrievedBeers = await _apiClient.GetAsync(gtAlcoholByVolume, ltAlcoholByVolume);
    }

    
}