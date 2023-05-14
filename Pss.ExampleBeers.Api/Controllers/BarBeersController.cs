using Microsoft.AspNetCore.Mvc;
using Pss.ExampleBeers.Api.Models;
using Pss.ExampleBeers.ApplicationServices;
using Pss.ExampleBeers.Models.Model.Bars;

namespace Pss.ExampleBeers.Api.Controllers;

[ApiController]
[Route("bar")]
public class BarBeersController : Controller
{
    private readonly IBarBeersService _barBeersService;

    public BarBeersController(IBarBeersService barBeersService)
    {
        _barBeersService = barBeersService ?? throw new ArgumentNullException(nameof(barBeersService));
    }

    [HttpPost]
    [Route("beer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(BarBeerLinkModel linkModel)
    {
        var (bar, beer) = await _barBeersService.LinkBeerAsync(linkModel.BarId, linkModel.BeerId);

        if (bar == null) return ValidationProblem($"Bar with ID {linkModel.BarId} could not be found.", statusCode: StatusCodes.Status400BadRequest);
        if (beer == null) return ValidationProblem($"Beer with ID {linkModel.BeerId} could not be found.", statusCode: StatusCodes.Status400BadRequest);

        return NoContent();
    }

    [HttpGet]
    [Route("{barId:guid}/beer")]
    [ProducesResponseType(typeof(BarWithBeers), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid barId)
    {
        var result = await _barBeersService.GetBarBeersAsync(barId);

        return result != null ? Ok(result) : NotFound();
    }

    [HttpGet]
    [Route("beer")]
    [ProducesResponseType(typeof(BarWithBeers), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get()
    {
        var result = await _barBeersService.GetBarBeersAsync();

        return result.Count > 0 ? Ok(result) : NoContent();
    }
}