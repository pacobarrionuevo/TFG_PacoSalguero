using TFG_Back.Models.Database.Entidades;

namespace TFG_Back.Models.Database.Repositorios
{
    public class ServiceRepository : Repository<Service>
    {
        public ServiceRepository(DBContext context) : base(context) { }
    }
}
