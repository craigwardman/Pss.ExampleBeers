using Microsoft.AspNetCore.Mvc;
using Pss.ExampleBeers.Api.Models;
using Pss.ExampleBeers.ApplicationServices;
using Pss.ExampleBeers.Models.Model.Bars;

namespace Pss.ExampleBeers.Api.Controllers;

[ApiController]
[Route("bar")]
public class BarController : Controller
{
    private readonly IBarService _barService;

    public BarController(IBarService barService)
    {
        _barService = barService ?? throw new ArgumentNullException(nameof(barService));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Post(BarRequestModel requestModel)
    {
        var bar = await _barService.CreateAsync(requestModel.Name, requestModel.Address);

        return CreatedAtAction(nameof(Get), new { id = bar.Id }, bar.Id);
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<Bar>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Get()
    {
        var breweries = await _barService.GetAsync();

        return breweries.Count > 0 ? Ok(breweries) : NoContent();
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Bar), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id)
    {
        var bar = await _barService.GetAsync(id);

        return bar != null ? Ok(bar) : NotFound();

    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Bar), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(Guid id, BarRequestModel requestModel)
    {
        var bar = await _barService.UpdateAsync(id, requestModel.Name, requestModel.Address);
        
        return bar != null ? Ok(bar) : NotFound();
    }
}