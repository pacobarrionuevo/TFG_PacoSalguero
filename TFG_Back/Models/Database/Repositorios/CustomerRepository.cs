using TFG_Back.Models.Database.Entidades;

namespace TFG_Back.Models.Database.Repositorios
{
    public class CustomerRepository : Repository<Customer>
    {
        public CustomerRepository(DBContext context) : base(context) { }
    }
}
