using Microsoft.AspNetCore.Mvc;
using Pss.ExampleBeers.Api.Models;
using Pss.ExampleBeers.ApplicationServices;
using Pss.ExampleBeers.Models.Model.Breweries;

namespace Pss.ExampleBeers.Api.Controllers;

[ApiController]
[Route("brewery")]
public class BreweryController : Controller
{
    private readonly IBreweryService _breweryService;

    public BreweryController(IBreweryService breweryService)
    {
        _breweryService = breweryService ?? throw new ArgumentNullException(nameof(breweryService));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Post(BreweryRequestModel requestModel)
    {
        var brewery = await _breweryService.CreateAsync(requestModel.Name);

        return CreatedAtAction(nameof(Get), new { id = brewery.Id }, brewery.Id);
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<Brewery>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Get()
    {
        var breweries = await _breweryService.GetAsync();

        return breweries.Count > 0 ? Ok(breweries) : NoContent();
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Brewery), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id)
    {
        var brewery = await _breweryService.GetAsync(id);

        return brewery != null ? Ok(brewery) : NotFound();

    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Brewery), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(Guid id, BreweryRequestModel requestModel)
    {
        var brewery = await _breweryService.UpdateAsync(id, requestModel.Name);
        
        return brewery != null ? Ok(brewery) : NotFound();
    }
}