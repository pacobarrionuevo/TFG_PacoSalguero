namespace TFG_Back.Models.DTO
{
    // DTO principal para representar a un usuario en la aplicación.
    // Se utiliza para enviar datos de usuario al cliente sin exponer información sensible como el hash de la contraseña.
    public class UserDTO
    {
        public int UserId { get; set; }
        public string UserProfilePhoto { get; set; }
        public string UserNickname { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
        public string UserConfirmPassword { get; set; }
        public bool IsOnline { get; set; }
        public DateTime? LastSeen { get; set; }

        public string Role { get; set; } = "usuario";
        // Propiedad para indicar si el usuario actual es amigo del usuario representado por este DTO.
        public bool IsFriend { get; set; }

    }
}