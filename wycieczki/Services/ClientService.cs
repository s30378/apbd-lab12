using Microsoft.EntityFrameworkCore;
using wycieczki.Data;

namespace wycieczki.Services;

public class ClientService : IClientService
{
    private readonly Apbd4Context _context;

    public ClientService(Apbd4Context context)
    {
        _context = context;
    }

    public async Task<bool> DeleteClientAsync(int idClient)
    {
        var client = await _context.Clients
            .Include(c => c.ClientTrips)
            .FirstOrDefaultAsync(c => c.IdClient == idClient);

        if (client == null)
            return false;

        if (client.ClientTrips.Any())
            throw new InvalidOperationException("Klient ma przypisane wycieczki, a więc nie można go usunąć.");

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
        return true;
    }
}