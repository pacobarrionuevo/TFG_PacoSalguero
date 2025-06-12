using TFG_Back.Models.Database.Entidades;

namespace TFG_Back.Models.Database.Repositorios
{
    // Repositorio específico para la entidad ServiceFacturado.
    public class ServiceFacturadoRepository : Repository<ServiceFacturado>
    {
        public ServiceFacturadoRepository(DBContext context) : base(context) { }
    }
}