using Pss.ExampleBeers.Api.Models;
using Pss.ExampleBeers.Domain.Model.Breweries;

namespace Pss.ExampleBeers.IntegrationTests.Features.Breweries;

public class ApiClient
{
    private readonly HttpClient _httpClient;

    public HttpResponseMessage Response { get; private set; } = new();

    public ApiClient(WebTestFixture webTestFixture)
    {
        _httpClient = webTestFixture.CreateClient();
    }

    public async Task PostAsync(string name)
    {
        var request = new BreweryRequestModel(name);
        Response = await _httpClient.PostAsJsonAsync("brewery", request);
    }

    public async Task<Brewery?> GetAsync(Guid id)
    {
        Response = await _httpClient.GetAsync($"brewery/{id}");
        if (Response.IsSuccessStatusCode)
        {
            return await Response.Content.ReadAsAsync<Brewery>();
        }

        return null;
    }

    public async Task<Brewery?> PutAsync(Guid id, string newName)
    {
        Response = await _httpClient.PutAsJsonAsync($"brewery/{id}", new BreweryRequestModel(newName));
        if (Response.IsSuccessStatusCode)
        {
            return await Response.Content.ReadAsAsync<Brewery>();
        }

        return null;
    }

    public async Task<IReadOnlyList<Brewery>> GetAsync()
    {
        Response = await _httpClient.GetAsync($"brewery");
        if (Response.IsSuccessStatusCode)
        {
            return await Response.Content.ReadAsAsync<List<Brewery>>();
        }

        return Array.Empty<Brewery>();
    }
}