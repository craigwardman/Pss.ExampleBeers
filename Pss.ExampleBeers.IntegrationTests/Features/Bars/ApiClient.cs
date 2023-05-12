using Pss.ExampleBeers.Api.Models;
using Pss.ExampleBeers.Domain.Model.Bars;

namespace Pss.ExampleBeers.IntegrationTests.Features.Bars;

public class ApiClient
{
    private readonly HttpClient _httpClient;

    public HttpResponseMessage Response { get; private set; } = new();

    public ApiClient(WebTestFixture webTestFixture)
    {
        _httpClient = webTestFixture.CreateClient();
    }

    public async Task PostAsync(string name, string address)
    {
        var request = new BarRequestModel(name, address);
        Response = await _httpClient.PostAsJsonAsync("bar", request);
    }

    public async Task<Bar?> GetAsync(Guid id)
    {
        Response = await _httpClient.GetAsync($"bar/{id}");
        if (Response.IsSuccessStatusCode)
        {
            return await Response.Content.ReadAsAsync<Bar>();
        }

        return null;
    }

    public async Task<Bar?> PutAsync(Guid id, string newName, string newAddress)
    {
        Response = await _httpClient.PutAsJsonAsync($"bar/{id}", new BarRequestModel(newName, newAddress));
        if (Response.IsSuccessStatusCode)
        {
            return await Response.Content.ReadAsAsync<Bar>();
        }

        return null;
    }

    public async Task<IReadOnlyList<Bar>> GetAsync()
    {
        Response = await _httpClient.GetAsync($"bar");
        if (Response.IsSuccessStatusCode)
        {
            return await Response.Content.ReadAsAsync<List<Bar>>();
        }

        return Array.Empty<Bar>();
    }
}