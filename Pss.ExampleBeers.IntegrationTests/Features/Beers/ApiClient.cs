using Pss.ExampleBeers.Api.Models;
using Pss.ExampleBeers.Domain.Model.Beers;

namespace Pss.ExampleBeers.IntegrationTests.Features.Beers;

public class ApiClient
{
    private readonly HttpClient _httpClient;

    public HttpResponseMessage Response { get; private set; } = new();

    public ApiClient(WebTestFixture webTestFixture)
    {
        _httpClient = webTestFixture.CreateClient();
    }

    public async Task PostAsync(string name, double percentageAlcoholByVolume)
    {
        var request = new BeerRequestModel(name, percentageAlcoholByVolume);
        Response = await _httpClient.PostAsJsonAsync("beer", request);
    }

    public async Task<Beer?> GetAsync(Guid id)
    {
        Response = await _httpClient.GetAsync($"beer/{id}");
        if (Response.IsSuccessStatusCode)
        {
            return await Response.Content.ReadAsAsync<Beer>();
        }

        return null;
    }

    public async Task<Beer?> PutAsync(Guid id, string newName, double newPercentageAlcoholByVolume)
    {
        Response = await _httpClient.PutAsJsonAsync($"beer/{id}", new BeerRequestModel(newName, newPercentageAlcoholByVolume));
        if (Response.IsSuccessStatusCode)
        {
            return await Response.Content.ReadAsAsync<Beer>();
        }

        return null;
    }

    public async Task<IReadOnlyList<Beer>> GetAsync()
    {
        Response = await _httpClient.GetAsync($"beer");
        if (Response.IsSuccessStatusCode)
        {
            return await Response.Content.ReadAsAsync<List<Beer>>();
        }

        return Array.Empty<Beer>();
    }

    public async Task<IReadOnlyList<Beer>> GetAsync(double gtAlcoholByVolume, double ltAlcoholByVolume)
    {
        Response = await _httpClient.GetAsync($"beer?gtAlcoholByVolume={gtAlcoholByVolume}&ltAlcoholByVolume={ltAlcoholByVolume}");
        if (Response.IsSuccessStatusCode)
        {
            return await Response.Content.ReadAsAsync<List<Beer>>();
        }

        return Array.Empty<Beer>();
    }
}