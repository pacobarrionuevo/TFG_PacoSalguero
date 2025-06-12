using TFG_Back.Models.Database.Repositorios;

namespace TFG_Back.Models.Database.Entidades
{
    // Entidad que representa a un usuario en la base de datos.
    public class User
    {
        public int UserId { get; set; }
        public string UserProfilePhoto { get; set; }
        public string UserNickname { get; set; }
        public string UserEmail { get; set; }
        // Almacena el hash de la contraseña.
        public string UserPassword { get; set; }
        public string UserConfirmPassword { get; set; }
        // Rol del usuario (ej. "user", "admin").
        public string Role { get; set; } = "user";
        // Relación muchos a muchos con FriendShip.
        public List<UserHasFriendship> UserFriendship { get; set; }

        // Indica si el usuario está actualmente conectado vía WebSocket.
        public bool IsOnline { get; set; } = false;
        // Almacena la fecha y hora de la última vez que el usuario estuvo conectado.
        public DateTime? LastSeen { get; set; }
    }
}