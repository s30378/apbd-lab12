using Microsoft.AspNetCore.Mvc;
using wycieczki.Dtos;
using wycieczki.Services;

namespace wycieczki.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly ITripService _tripService;

    public TripsController(ITripService tripService)
    {
        _tripService = tripService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTrips([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _tripService.GetTripsAsync(page, pageSize);
        return Ok(result);
    }

    [HttpPost("{idTrip}/clients")]
    public async Task<IActionResult> RegisterClientToTrip(int idTrip, RegisterClientDto registerClientDto)
    {
        try
        {
            await _tripService.RegisterClientToTripAsync(idTrip, registerClientDto);
            return Ok("Klient zapisany na wycieczkę.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}