namespace TFG_Back.Models.DTO
{
    public class CustomerDTO
    {
        public int Id { get; set; }
        public int CIF { get; set; }
        public string Name { get; set; }
        public string Adress { get; set; }
        public int PostalCode { get; set; }
        public string PlaceOfResidence { get; set; }
        public int PhoneNumber { get; set; }
        public string Email { get; set; }
        public string AdminEmail { get; set; }

        public int PaymentMethodId { get; set; }
    }

}
