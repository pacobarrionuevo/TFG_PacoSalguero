using TFG_Back.Models.Database.Entidades;

namespace TFG_Back.Models.Database.Repositorios
{
    // Repositorio específico para la entidad PaymentMethod.
    public class PaymentMethodRepository : Repository<PaymentMethod>
    {
        public PaymentMethodRepository(DBContext context) : base(context) { }
    }
}