using TFG_Back.Models.Database.Entidades;

namespace TFG_Back.Models.Database.Repositorios
{
    public class ServiceFacturadoRepository : Repository<ServiceFacturado>
    {
        public ServiceFacturadoRepository(DBContext context) : base(context) { }
    }

}
