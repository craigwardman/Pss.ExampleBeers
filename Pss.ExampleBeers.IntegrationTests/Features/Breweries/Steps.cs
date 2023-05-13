using System.Diagnostics;
using System.Net;
using FluentAssertions;
using FluentAssertions.Execution;
using Pss.ExampleBeers.Api.Models;
using Pss.ExampleBeers.Models.Model.Beers;
using Pss.ExampleBeers.Models.Model.Breweries;
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
    private IReadOnlyList<BreweryWithBeers> _retrievedBreweriesWithBeers = Array.Empty<BreweryWithBeers>();
    private BreweryWithBeers? _retrievedBreweryWithBeers;

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

    [When(@"I send a request to create link '(.*)' with brewery '(.*)'")]
    public async Task WhenISendARequestToCreateLinkWithBrewery(string beerName, string breweryName)
    {
        var breweryId =
            TestDataStore.Repository<Brewery>(BreweriesCollection.Name).Items?
                .SingleOrDefault(i => i.Name == breweryName)?.Id ?? Guid.NewGuid();
        var beerId =
            TestDataStore.Repository<Beer>(BeersCollection.Name).Items?.SingleOrDefault(i => i.Name == beerName)?.Id ??
            Guid.NewGuid();

        await _apiClient.LinkBeerAsync(breweryId, beerId);
    }

    [Then(@"I get an unsuccessful breweries response, with status '(.*)'")]
    public void ThenIGetAnUnsuccessfulBreweriesResponseWithStatus(HttpStatusCode statusCode)
    {
        using (new AssertionScope())
        {
            _apiClient.Response.IsSuccessStatusCode.Should().BeFalse();
            _apiClient.Response.StatusCode.Should().Be(statusCode);
        }
    }

    [Given(@"the below brewery/beer links exist in the database")]
    public void GivenTheBelowBreweryBeerLinksExistInTheDatabase(Table table)
    {
        var breweries = new Dictionary<string, Brewery>();
        var beers = new Dictionary<string, Beer>();

        var breweryBeers = new List<BreweryBeer>();

        foreach (var tableRow in table.Rows)
        {
            if (!breweries.TryGetValue(tableRow["Brewery"], out var brewery))
            {
                brewery = Brewery.Create(tableRow["Brewery"]);
                breweries.Add(brewery.Name, brewery);
            }

            if (!beers.TryGetValue(tableRow["Beer"], out var beer))
            {
                beer = Beer.Create(tableRow["Beer"], 5);
                beers.Add(beer.Name, beer);
            }

            breweryBeers.Add(new BreweryBeer(brewery.Id, beer.Id));
        }

        TestDataStore.Repository<Brewery>(BreweriesCollection.Name).Items = breweries.Values.ToArray();
        TestDataStore.Repository<Beer>(BeersCollection.Name).Items = beers.Values.ToArray();
        TestDataStore.Repository<BreweryBeer>(BreweryBeersCollection.Name).Items = breweryBeers;
    }

    [When(@"I request the '(.*)' brewery beers")]
    public async Task WhenIRequestTheBreweryBeers(string breweryName)
    {
        var id = TestDataStore.Repository<Brewery>(BreweriesCollection.Name)
            .Items.Single(i => i.Name == breweryName)
            .Id;

        _retrievedBreweryWithBeers = await _apiClient.GetBeersAsync(id);
    }

    [Then(@"the returned brewery beers list is below")]
    public void ThenTheReturnedBreweryBeersListIsBelow(Table table)
    {
        _retrievedBreweriesWithBeers.Should().NotBeEmpty();

        var results = _retrievedBreweriesWithBeers.Select(b => new
            { BreweryName = b.Brewery.Name, Beers = string.Join(", ", b.Beers.Select(beer => beer.Name)) });

        var expected = table.Rows.Select(r => new { BreweryName = r["Brewery"], Beers = r["Beers"] });

        results.Should().BeEquivalentTo(expected);
    }

    [Then(@"the returned '(.*)' brewery beers list is '(.*)'")]
    public void ThenTheReturnedBreweryBeersListIs(string breweryName, string beers)
    {
        using (new AssertionScope())
        {
            Debug.Assert(_retrievedBreweryWithBeers != null);

            _retrievedBreweryWithBeers.Brewery.Name.Should().Be(breweryName);
            string.Join(", ", _retrievedBreweryWithBeers.Beers.Select(b => b.Name)).Should().Be(beers);
        }
    }

    [When(@"I request the all brewery beers")]
    public async Task WhenIRequestTheAllBreweryBeers()
    {
        _retrievedBreweriesWithBeers = await _apiClient.GetBeersAsync();
    }
}