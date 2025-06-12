namespace TFG_Back.Models.DTO
{
    // DTO para el registro de un nuevo usuario.
    // Incluye el archivo de la foto de perfil para ser procesado en el backend.
    public class UserSignUpDTO
    {
        public IFormFile UserProfilePhoto { get; set; }
        public string UserNickname { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
        public string UserConfirmPassword { get; set; }
    }
}