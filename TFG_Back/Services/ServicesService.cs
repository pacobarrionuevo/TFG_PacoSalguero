namespace TFG_Back.Services;

using TFG_Back.Models.Database.Entidades;
using TFG_Back.Models.Database.Repositorios;

public class ServicesService
{
    private readonly ServiceRepository _repository;

    public ServicesService(ServiceRepository repository)
    {
        _repository = repository;
    }

    public async Task<ICollection<Service>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Service> CreateAsync(Service servicio)
    {
        var result = await _repository.InsertAsync(servicio);
        return result;
    }
}
