namespace TFG_Back.Models.DTO
{
    // DTO para la creación o actualización de una imagen, incluyendo el archivo y su nombre.
    public class CreateUpdateImageRequest
    {
        public string Name { get; set; }
        public IFormFile File { get; set; }
    }
}