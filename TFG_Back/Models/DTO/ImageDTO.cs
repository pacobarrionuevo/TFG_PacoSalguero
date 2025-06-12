namespace TFG_Back.Models.DTO
{
    // DTO para representar una imagen, incluyendo su URL completa para el cliente.
    public class ImageDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}