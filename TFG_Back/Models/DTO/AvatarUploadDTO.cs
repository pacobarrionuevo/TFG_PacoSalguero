using Microsoft.AspNetCore.Http;

namespace TFG_Back.Models.DTO
{
    // DTO para manejar la subida de un archivo de avatar desde un formulario.
    public class AvatarUploadDTO
    {
        public IFormFile Avatar { get; set; }
    }
}