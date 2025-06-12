namespace TFG_Back.Models.Database.Entidades
{
    // Entidad que representa una imagen almacenada en el sistema.
    public class Image
    {
        public long Id { get; set; }
        public string Name { get; set; }
        // Almacena la ruta relativa al archivo en el servidor.
        public string Path { get; set; }
    }
}