using TFG_Back.Models.Database.Entidades;

namespace TFG_Back.Models.Database.Repositorios
{
    public class PaymentMethodRepository: Repository<PaymentMethod>
    {
        public PaymentMethodRepository(DBContext context) : base(context) { }
    }
}
