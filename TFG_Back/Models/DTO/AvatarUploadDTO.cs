using Microsoft.AspNetCore.Http;

namespace TFG_Back.Models.DTO
{
    public class AvatarUploadDTO
    {
        public IFormFile Avatar { get; set; }
    }
}