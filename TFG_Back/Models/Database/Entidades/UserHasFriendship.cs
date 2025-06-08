namespace TFG_Back.Models.Database.Entidades
{
    public class UserHasFriendship
    {
        public int UserId { get; set; }
        public int FriendshipId { get; set; }
        public int UserHasFriendshipId { get; set; }

        public bool Requestor { get; set; }

        public User User { get; set; }
        public FriendShip Friendship { get; set; }
    }
}
