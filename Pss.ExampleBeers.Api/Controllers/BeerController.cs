using Microsoft.AspNetCore.Mvc;
using Pss.ExampleBeers.Api.Models;
using Pss.ExampleBeers.ApplicationServices;
using Pss.ExampleBeers.Models.Model.Beers;

namespace Pss.ExampleBeers.Api.Controllers;

[ApiController]
[Route("beer")]
public class BeerController : Controller
{
    private readonly IBeerService _beerService;

    public BeerController(IBeerService beerService)
    {
        _beerService = beerService ?? throw new ArgumentNullException(nameof(beerService));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Post(BeerRequestModel requestModel)
    {
        var beer = await _beerService.CreateAsync(requestModel.Name, requestModel.PercentageAlcoholByVolume);

        return CreatedAtAction(nameof(Get), new { id = beer.Id }, beer.Id);
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<Beer>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Get(double? gtAlcoholByVolume, double? ltAlcoholByVolume)
    {
        var beers = await _beerService.GetAsync(gtAlcoholByVolume, ltAlcoholByVolume);

        return beers.Count > 0 ? Ok(beers) : NoContent();
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Beer), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id)
    {
        var beer = await _beerService.GetAsync(id);

        return beer != null ? Ok(beer) : NotFound();

    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Beer), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(Guid id, BeerRequestModel requestModel)
    {
        var beer = await _beerService.UpdateAsync(id, requestModel.Name, requestModel.PercentageAlcoholByVolume);
        
        return beer != null ? Ok(beer) : NotFound();
    }
}