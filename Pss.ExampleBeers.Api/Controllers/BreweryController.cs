using Microsoft.AspNetCore.Mvc;
using Pss.ExampleBeers.Api.Models;
using Pss.ExampleBeers.ApplicationServices;

namespace Pss.ExampleBeers.Api.Controllers;

[ApiController]
[Route("brewery")]
public class BreweryController : Controller
{
    private readonly IBreweryService _breweryService;

    public BreweryController(IBreweryService breweryService)
    {
        _breweryService = breweryService;
    }

    [HttpPost]
    public async Task<IActionResult> Post(BreweryRequestModel requestModel)
    {
        var brewery = await _breweryService.CreateAsync(requestModel.Name);

        return CreatedAtAction(nameof(Get), new { id = brewery.Id }, brewery.Id);
    }
    
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var breweries = await _breweryService.GetAsync();

        return breweries.Count > 0 ? Ok(breweries) : NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var brewery = await _breweryService.GetAsync(id);

        return brewery != null ? Ok(brewery) : NotFound();

    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Put(Guid id, BreweryRequestModel requestModel)
    {
        var brewery = await _breweryService.UpdateAsync(id, requestModel.Name);
        
        return brewery != null ? Ok(brewery) : NotFound();
    }
}