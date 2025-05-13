using System.Text.Json.Serialization;

namespace TFG_Back.Models.Database.Entidades
{
    public class PaymentMethod
    {
        public int Id { get; set; }
        public string Method { get; set; }
        public int Installments { get; set; }
        public int FirstPaymentDays { get; set; }
        public int DaysBetweenPayments { get; set; }

        // Clientes que usan cierto método de pago
        // Se ignora porque solo lo vamos a usar para completar la relación entre esta entidad y Customer
        [JsonIgnore]
        public ICollection<Customer> Customers { get; set; } = new List<Customer>();
    }
}
