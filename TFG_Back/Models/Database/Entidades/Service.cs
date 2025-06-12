namespace TFG_Back.Models.Database.Entidades
{
    // Entidad que representa un tipo de servicio ofrecido.
    public class Service
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Abreviatura { get; set; } = null!;
        // Color asociado al servicio para una mejor visualización en el frontend.
        public string Color { get; set; } = null!;
    }
}