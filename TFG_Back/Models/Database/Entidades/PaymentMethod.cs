namespace TFG_Back.Models.Database.Entidades
{
    public class PaymentMethod
    {
        public int Id { get; set; }
        public string Method { get; set; }
        public int Installments { get; set; }
        public int FirstPaymentDays { get; set; }
        public int DaysBetweenPayments { get; set; }
    }
}
