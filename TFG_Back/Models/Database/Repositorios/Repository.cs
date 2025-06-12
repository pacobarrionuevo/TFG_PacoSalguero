using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;

namespace TFG_Back.Models.Database.Repositorios
{
    // Implementación genérica del patrón Repositorio.
    // Proporciona una abstracción sobre el DbContext para realizar operaciones CRUD.
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly DBContext _context;

        public Repository(DBContext context)
        {
            _context = context;
        }

        // Obtiene todas las entidades de un tipo.
        public async Task<ICollection<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToArrayAsync();
        }

        // Devuelve un IQueryable para construir consultas más complejas.
        // `asNoTracking` mejora el rendimiento para operaciones de solo lectura.
        public IQueryable<TEntity> GetQueryable(bool asNoTracking = true)
        {
            DbSet<TEntity> entities = _context.Set<TEntity>();
            return asNoTracking ? entities.AsNoTracking() : entities;
        }

        // Obtiene una entidad por su ID.
        public async Task<TEntity> GetByIdAsync(object id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        // Inserta una nueva entidad.
        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            EntityEntry<TEntity> entry = await _context.Set<TEntity>().AddAsync(entity);
            return entry.Entity;
        }

        // Actualiza una entidad existente.
        public TEntity Update(TEntity entity)
        {
            EntityEntry<TEntity> entry = _context.Set<TEntity>().Update(entity);
            return entry.Entity;
        }

        // Elimina una entidad.
        public void Delete(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }

        // Comprueba si una entidad con un ID específico existe.
        public async Task<bool> ExistsAsync(object id)
        {
            return await GetByIdAsync(id) != null;
        }
    }
}