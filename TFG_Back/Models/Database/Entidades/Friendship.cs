namespace TFG_Back.Models.Database.Entidades
{
    // Entidad que representa una relación de amistad entre dos usuarios.
    public class FriendShip
    {
        public int FriendShipId { get; set; }
        // Indica si la solicitud de amistad ha sido aceptada.
        public bool IsAccepted { get; set; } = false;
        // Relación muchos a muchos a través de la tabla intermedia UserHasFriendship.
        public List<UserHasFriendship> UserFriendship { get; set; }
    }
}