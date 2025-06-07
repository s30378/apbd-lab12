namespace wycieczki.Services;

public interface IClientService
{
    Task<bool> DeleteClientAsync(int idClient);
}