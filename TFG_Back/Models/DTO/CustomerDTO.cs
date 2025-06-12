namespace TFG_Back.Models.DTO
{
    // DTO para la creación y actualización de clientes.
    // Separa la lógica de la entidad de la base de datos.
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

        // Se utiliza el ID para la relación, simplificando la creación/actualización.
        public int PaymentMethodId { get; set; }
    }

}