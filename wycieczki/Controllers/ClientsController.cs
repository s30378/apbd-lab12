using Microsoft.AspNetCore.Mvc;
using wycieczki.Services;

namespace wycieczki.Controllers;

[ApiController]
[Route("api/clients")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpDelete("{idClient}")]
    public async Task<IActionResult> DeleteClient(int idClient)
    {
        try
        {
            var result = await _clientService.DeleteClientAsync(idClient);

            if (!result)
                return NotFound($"Klient o ID {idClient} nie istnieje.");

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}