using System.Diagnostics;
using FluentAssertions;
using Pss.ExampleBeers.Api.Models;
using Pss.ExampleBeers.Domain.Model.Bars;
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
}