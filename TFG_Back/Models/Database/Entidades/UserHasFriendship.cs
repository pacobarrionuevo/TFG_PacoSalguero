namespace TFG_Back.Models.Database.Entidades
{
    // Tabla intermedia para la relación muchos a muchos entre User y FriendShip.
    public class UserHasFriendship
    {
        public int UserId { get; set; }
        public int FriendshipId { get; set; }
        public int UserHasFriendshipId { get; set; }

        // Indica si este usuario fue el que envió la solicitud de amistad.
        public bool Requestor { get; set; }

        // Propiedades de navegación a las entidades relacionadas.
        public User User { get; set; }
        public FriendShip Friendship { get; set; }
    }
}