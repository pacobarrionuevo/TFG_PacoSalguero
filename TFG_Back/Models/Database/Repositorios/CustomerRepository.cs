using TFG_Back.Models.Database.Entidades;

namespace TFG_Back.Models.Database.Repositorios
{
    // Repositorio específico para la entidad Customer.
    // Hereda la funcionalidad CRUD genérica de la clase Repository.
    public class CustomerRepository : Repository<Customer>
    {
        public CustomerRepository(DBContext context) : base(context) { }
    }
}