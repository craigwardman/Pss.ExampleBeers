using System.Diagnostics;
using FluentAssertions;
using Pss.ExampleBeers.Api.Models;
using Pss.ExampleBeers.Domain.Model.Breweries;
using Pss.ExampleBeers.MongoDB.Mongo;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using TestDataDefinitionFramework.Core;

namespace Pss.ExampleBeers.IntegrationTests.Features.Breweries;

[Binding]
public class Steps
{
    private readonly ApiClient _apiClient;
    private Brewery? _retrievedBrewery;
    private IReadOnlyList<Brewery> _retrievedBreweries = Array.Empty<Brewery>();

    public Steps(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    [When(@"I send a request to create a brewery called '(.*)'")]
    public async Task WhenISendARequestToCreateABreweryCalledThatIsByVolume(string name)
    {
        await _apiClient.PostAsync(name);
    }

    [Then(@"I get a successful breweries response")]
    public void ThenIGetASuccessfulBreweriesResponse()
    {
        _apiClient.Response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Then(@"my new brewery is given an ID")]
    public void ThenMyNewBreweryIsGivenAnId()
    {
        _apiClient.Response.Content.ReadAsAsync<Guid>().Should().NotBeSameAs(Guid.Empty);
    }

    [When(@"I request the '(.*)' brewery by it's ID")]
    public async Task WhenIRequestTheBreweryByItsId(string name)
    {
        var id = TestDataStore.Repository<Brewery>(BreweriesCollection.Name).Items.Single(i => i.Name == name).Id;
        _retrievedBrewery = await _apiClient.GetAsync(id);
    }

    [Then(@"the returned brewery is called '(.*)'")]
    public void ThenTheReturnedBreweryIsCalledThatIsByVolume(string name)
    {
        Debug.Assert(_retrievedBrewery != null);

        _retrievedBrewery.Name.Should().Be(name);
    }

    [When(@"I request to update the '(.*)' brewery to be called '(.*)'")]
    public async Task WhenIRequestToUpdateTheBreweryToBeCalledThatIsByVolume(string name, string newName)
    {
        var id = TestDataStore.Repository<Brewery>(BreweriesCollection.Name).Items.Single(i => i.Name == name).Id;
        _retrievedBrewery = await _apiClient.PutAsync(id, newName);
    }

    [When(@"I request all the breweries")]
    public async Task WhenIRequestAllTheBreweries()
    {
        _retrievedBreweries = await _apiClient.GetAsync();
    }

    [Then(@"the returned list of breweries is as below")]
    public void ThenTheReturnedListOfBreweriesIsAsBelow(Table table)
    {
        var expected = table.CreateSet<BreweryRequestModel>().ToList();

        _retrievedBreweries.Should().BeEquivalentTo(expected);
    }
}