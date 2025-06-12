namespace TFG_Back.Models.Database.Entidades
{
    // Entidad que representa a un cliente en la base de datos.
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
        public string AdminEmail { get; set; }

        // Clave foránea para la relación con PaymentMethod.
        public int PaymentMethodId { get; set; }
        // Propiedad de navegación para acceder al método de pago asociado.
        public PaymentMethod PaymentMethod { get; set; }
    }
}