using System.Diagnostics;
using System.Net;
using FluentAssertions;
using FluentAssertions.Execution;
using Pss.ExampleBeers.Api.Models;
using Pss.ExampleBeers.Models.Model.Bars;
using Pss.ExampleBeers.Models.Model.Beers;
using Pss.ExampleBeers.MongoDB.Mongo;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using TestDataDefinitionFramework.Core;

namespace Pss.ExampleBeers.IntegrationTests.Features.Bars;

[Binding]
public class Steps
{
    private readonly ApiClient _apiClient;
    private Bar? _retrievedBar;
    private IReadOnlyList<Bar> _retrievedBars = Array.Empty<Bar>();
    private IReadOnlyList<BarWithBeers> _retrievedBarsWithBeers = Array.Empty<BarWithBeers>();
        private BarWithBeers? _retrievedBarWithBeers;

    public Steps(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    [When(@"I send a request to create a bar called '(.*)' at address '(.*)'")]
    public async Task WhenISendARequestToCreateABarCalledThatIsByVolume(string name, string address)
    {
        await _apiClient.PostAsync(name, address);
    }

    [Then(@"I get a successful bars response")]
    public void ThenIGetASuccessfulBarsResponse()
    {
        _apiClient.Response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Then(@"my new bar is given an ID")]
    public void ThenMyNewBarIsGivenAnId()
    {
        _apiClient.Response.Content.ReadAsAsync<Guid>().Should().NotBeSameAs(Guid.Empty);
    }

    [When(@"I request the '(.*)' bar by it's ID")]
    public async Task WhenIRequestTheBarByItsId(string name)
    {
        var id = TestDataStore.Repository<Bar>(BarsCollection.Name).Items.Single(i => i.Name == name).Id;
        _retrievedBar = await _apiClient.GetAsync(id);
    }

    [Then(@"the returned bar is called '(.*)'")]
    public void ThenTheReturnedBarIsCalledThatIsByVolume(string name)
    {
        Debug.Assert(_retrievedBar != null);

        _retrievedBar.Name.Should().Be(name);
    }

    [When(@"I request to update the '(.*)' bar to be called '(.*)' now at address '(.*)'")]
    public async Task WhenIRequestToUpdateTheBarToBeCalledThatIsByVolume(string name, string newName, string newAddress)
    {
        var id = TestDataStore.Repository<Bar>(BarsCollection.Name).Items.Single(i => i.Name == name).Id;
        _retrievedBar = await _apiClient.PutAsync(id, newName, newAddress);
    }

    [When(@"I request all the bars")]
    public async Task WhenIRequestAllTheBars()
    {
        _retrievedBars = await _apiClient.GetAsync();
    }

    [Then(@"the returned list of bars is as below")]
    public void ThenTheReturnedListOfBarsIsAsBelow(Table table)
    {
        var expected = table.CreateSet<BarRequestModel>().ToList();

        _retrievedBars.Should().BeEquivalentTo(expected);
    }
    
    [When(@"I send a request to create link '(.*)' with bar '(.*)'")]
    public async Task WhenISendARequestToCreateLinkWithBar(string beerName, string barName)
    {
        var barId =
            TestDataStore.Repository<Bar>(BarsCollection.Name).Items?
                .SingleOrDefault(i => i.Name == barName)?.Id ?? Guid.NewGuid();
        var beerId =
            TestDataStore.Repository<Beer>(BeersCollection.Name).Items?.SingleOrDefault(i => i.Name == beerName)?.Id ??
            Guid.NewGuid();

        await _apiClient.LinkBeerAsync(barId, beerId);
    }

    [Then(@"I get an unsuccessful bars response, with status '(.*)'")]
    public void ThenIGetAnUnsuccessfulBarsResponseWithStatus(HttpStatusCode statusCode)
    {
        using (new AssertionScope())
        {
            _apiClient.Response.IsSuccessStatusCode.Should().BeFalse();
            _apiClient.Response.StatusCode.Should().Be(statusCode);
        }
    }

    [Given(@"the below bar/beer links exist in the database")]
    public void GivenTheBelowBarBeerLinksExistInTheDatabase(Table table)
    {
        var bars = new Dictionary<string, Bar>();
        var beers = new Dictionary<string, Beer>();

        var barBeers = new List<BarBeer>();

        foreach (var tableRow in table.Rows)
        {
            if (!bars.TryGetValue(tableRow["Bar"], out var bar))
            {
                bar = Bar.Create(tableRow["Bar"], "address");
                bars.Add(bar.Name, bar);
            }

            if (!beers.TryGetValue(tableRow["Beer"], out var beer))
            {
                beer = Beer.Create(tableRow["Beer"], 5);
                beers.Add(beer.Name, beer);
            }

            barBeers.Add(new BarBeer(bar.Id, beer.Id));
        }

        TestDataStore.Repository<Bar>(BarsCollection.Name).Items = bars.Values.ToArray();
        TestDataStore.Repository<Beer>(BeersCollection.Name).Items = beers.Values.ToArray();
        TestDataStore.Repository<BarBeer>(BarBeersCollection.Name).Items = barBeers;
    }

    [When(@"I request the '(.*)' bar beers")]
    public async Task WhenIRequestTheBarBeers(string barName)
    {
        var id = TestDataStore.Repository<Bar>(BarsCollection.Name)
            .Items.Single(i => i.Name == barName)
            .Id;

        _retrievedBarWithBeers = await _apiClient.GetBeersAsync(id);
    }

    [Then(@"the returned bar beers list is below")]
    public void ThenTheReturnedBarBeersListIsBelow(Table table)
    {
        _retrievedBarsWithBeers.Should().NotBeEmpty();

        var results = _retrievedBarsWithBeers.Select(b => new
            { BarName = b.Bar.Name, Beers = string.Join(", ", b.Beers.Select(beer => beer.Name)) });

        var expected = table.Rows.Select(r => new { BarName = r["Bar"], Beers = r["Beers"] });

        results.Should().BeEquivalentTo(expected);
    }

    [Then(@"the returned '(.*)' bar beers list is '(.*)'")]
    public void ThenTheReturnedBarBeersListIs(string barName, string beers)
    {
        using (new AssertionScope())
        {
            Debug.Assert(_retrievedBarWithBeers != null);

            _retrievedBarWithBeers.Bar.Name.Should().Be(barName);
            string.Join(", ", _retrievedBarWithBeers.Beers.Select(b => b.Name)).Should().Be(beers);
        }
    }

    [When(@"I request the all bar beers")]
    public async Task WhenIRequestTheAllBarBeers()
    {
        _retrievedBarsWithBeers = await _apiClient.GetBeersAsync();
    }
}