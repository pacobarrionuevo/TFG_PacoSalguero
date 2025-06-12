namespace TFG_Back.Services;

using TFG_Back.Models.Database.Entidades;
using TFG_Back.Models.Database.Repositorios;

// Servicio para la lógica de negocio de los servicios.
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
    public async Task<Service> UpdateAsync(Service servicio)
    {
        var existing = await _repository.GetByIdAsync(servicio.Id);
        if (existing == null) return null;

        existing.Nombre = servicio.Nombre;
        existing.Abreviatura = servicio.Abreviatura;
        existing.Color = servicio.Color;

        _repository.Update(existing);
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        _repository.Delete(entity);
        return true;
    }
}