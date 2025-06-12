namespace TFG_Back.Models.DTO
{
    // DTO para el proceso de inicio de sesión.
    // Permite iniciar sesión con email o nickname.
    public class UserLoginDTO
    {
        public string UserEmailOrNickname { get; set; }
        public string UserPassword { get; set; }
    }
}