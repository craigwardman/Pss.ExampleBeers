using Microsoft.AspNetCore.Mvc;
using Pss.ExampleBeers.Api.Models;
using Pss.ExampleBeers.ApplicationServices;
using Pss.ExampleBeers.Models.Model;
using Pss.ExampleBeers.Models.Model.Breweries;

namespace Pss.ExampleBeers.Api.Controllers;

[ApiController]
[Route("brewery")]
public class BreweryBeersController : Controller
{
    private readonly IBreweryBeersService _breweryBeersService;

    public BreweryBeersController(IBreweryBeersService breweryBeersService)
    {
        _breweryBeersService = breweryBeersService ?? throw new ArgumentNullException(nameof(breweryBeersService));
    }

    [HttpPost]
    [Route("beer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(BreweryBeerLinkModel linkModel)
    {
        var (brewery, beer) = await _breweryBeersService.LinkBeerAsync(linkModel.BreweryId, linkModel.BeerId);

        if (brewery == null) return ValidationProblem($"Brewery with ID {linkModel.BreweryId} could not be found.", statusCode: StatusCodes.Status400BadRequest);
        if (beer == null) return ValidationProblem($"Beer with ID {linkModel.BeerId} could not be found.", statusCode: StatusCodes.Status400BadRequest);

        return NoContent();
    }

    [HttpGet]
    [Route("{breweryId:guid}/beer")]
    [ProducesResponseType(typeof(BreweryWithBeers), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid breweryId)
    {
        var result = await _breweryBeersService.GetBreweryBeersAsync(breweryId);

        return result != null ? Ok(result) : NotFound();
    }
    
    [HttpGet]
    [Route("beer")]
    [ProducesResponseType(typeof(BreweryWithBeers), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get()
    {
        var result = await _breweryBeersService.GetBreweryBeersAsync();

        return result.Count > 0 ? Ok(result) : NoContent();
    }
}