namespace TFG_Back.Models.Database.Entidades
{
    public class Customer
    {
        public int Id { get; set; }
        public int CIF { get; set; }
        public string Name { get; set; }
        public string Adress { get; set; }
        public int PostalCode { get; set; }
        public string PlaceOfResidence { get; set; }
        public int PhoneNumber { get; set; }
        public string Email { get; set; }
        public string AdminisEmail { get; set; }

        public int PaymentMethodId { get; set; } //FK
        public PaymentMethod PaymentMethod { get; set; }


    }
}
