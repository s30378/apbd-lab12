using Microsoft.EntityFrameworkCore;
using wycieczki.Data;
using wycieczki.Dtos;
using wycieczki.Models;

namespace wycieczki.Services;

public class TripService : ITripService
{
    private readonly Apbd4Context _context;

    public TripService(Apbd4Context context)
    {
        _context = context;
    }

    public async Task<TripResponseDto> GetTripsAsync(int page, int pageSize)
    {
        var tripsCount = await _context.Trips.CountAsync();
        var totalPages = (int)Math.Ceiling(tripsCount / (double)pageSize);

        var trips = await _context.Trips
            .OrderByDescending(t => t.DateFrom)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(t => t.IdCountries)
            .Include(t => t.ClientTrips).ThenInclude(ct => ct.IdClientNavigation)
            .Select(t => new TripDto
            {
                Name = t.Name,
                Description = t.Description,
                DateFrom = t.DateFrom,
                DateTo = t.DateTo,
                MaxPeople = t.MaxPeople,
                Countries = t.IdCountries
                    .Select(c => new CountryDto { Name = c.Name })
                    .ToList(),
                Clients = t.ClientTrips
                    .Select(ct => new ClientDto
                    {
                        FirstName = ct.IdClientNavigation.FirstName,
                        LastName = ct.IdClientNavigation.LastName
                    })
                    .ToList()
            })
            .ToListAsync();

        return new TripResponseDto
        {
            PageNum = page,
            PageSize = pageSize,
            AllPages = totalPages,
            Trips = trips
        };
    }

    public async Task RegisterClientToTripAsync(int idTrip, RegisterClientDto registerClientDto)
    {
        var trip = await _context.Trips
            .FirstOrDefaultAsync(t => t.IdTrip == idTrip);

        if (trip == null || trip.DateFrom <= DateTime.Now)
            throw new InvalidOperationException("Wycieczka nie istnieje lub już się odbyła.");

        var existingClient = await _context.Clients
            .Include(c => c.ClientTrips)
            .FirstOrDefaultAsync(c => c.Pesel == registerClientDto.Pesel);

        if (existingClient != null)
        {
            var alreadyAssigned = await _context.ClientTrips
                .AnyAsync(ct => ct.IdClient == existingClient.IdClient && ct.IdTrip == idTrip);

            if (alreadyAssigned)
                throw new InvalidOperationException("Klient już zapisany na tę wycieczkę.");

            throw new InvalidOperationException("Klient o podanym numerze PESEL już istnieje.");
        }

        var client = new Client
        {
            FirstName = registerClientDto.FirstName,
            LastName = registerClientDto.LastName,
            Email = registerClientDto.Email,
            Telephone = registerClientDto.Telephone,
            Pesel = registerClientDto.Pesel
        };

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        _context.ClientTrips.Add(new ClientTrip
        {
            IdClient = client.IdClient,
            IdTrip = idTrip,
            RegisteredAt = DateTime.Now,
            PaymentDate = registerClientDto.PaymentDate
        });

        await _context.SaveChangesAsync();
    }
}