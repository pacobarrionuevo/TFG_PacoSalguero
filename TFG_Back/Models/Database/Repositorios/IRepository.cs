namespace TFG_Back.Models.Database.Repositorios
{
    // Interfaz genérica para el patrón Repositorio.
    // Define las operaciones CRUD estándar que cualquier repositorio debe implementar.
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<ICollection<TEntity>> GetAllAsync();
        IQueryable<TEntity> GetQueryable(bool asNoTracking = true);
        Task<TEntity> GetByIdAsync(object id);
        Task<TEntity> InsertAsync(TEntity entity);
        TEntity Update(TEntity entity);
        void Delete(TEntity entity);
        Task<bool> ExistsAsync(object id);
    }
}