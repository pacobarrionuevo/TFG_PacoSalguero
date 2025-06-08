namespace TFG_Back.Models.Database.Entidades
{
    public class UserHasFriendship
    {
        public int UserId { get; set; }
        public int FriendshipId { get; set; }
        public int UsuarioTieneAmistadId { get; set; }

        public bool Requestor { get; set; }

        public User User { get; set; }
        public Friendship Friendship { get; set; }
    }
}
