namespace TFG_Back.Models.DTO
{
    public class CustomerDTO
    {
        public int CIF { get; set; }
        public string Name { get; set; }
        public string Adress { get; set; }
        public int PostalCode { get; set; }
        public string PlaceOfResidence { get; set; }
        public int PhoneNumber { get; set; }
        public string Email { get; set; }
        public string AdminisEmail { get; set; }

        public string PaymentMethodName { get; set; } // En vez del objeto entero
    }

}
