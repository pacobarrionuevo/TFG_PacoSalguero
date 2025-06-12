using System.Text.Json.Serialization;

namespace TFG_Back.Models.Database.Entidades
{
    // Entidad que representa un método de pago.
    public class PaymentMethod
    {
        public int Id { get; set; }
        public string Method { get; set; }
        public int Installments { get; set; }
        public int FirstPaymentDays { get; set; }
        public int DaysBetweenPayments { get; set; }

        // Colección de clientes que usan este método de pago.
        // Se ignora en la serialización JSON para evitar bucles de referencia.
        [JsonIgnore]
        public ICollection<Customer> Customers { get; set; } = new List<Customer>();
    }
}