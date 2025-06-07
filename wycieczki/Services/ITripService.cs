using wycieczki.Dtos;

namespace wycieczki.Services;

public interface ITripService
{
    Task<TripResponseDto> GetTripsAsync(int page, int pageSize);
    Task RegisterClientToTripAsync(int idTrip, RegisterClientDto registerClientDto);
}