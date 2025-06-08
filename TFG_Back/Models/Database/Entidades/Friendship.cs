namespace TFG_Back.Models.Database.Entidades
{
    public class Friendship
    {
        public int FriendShipId { get; set; }
        public bool IsAccepted { get; set; } = false;
        public List<UserHasFriendship> UserFriendship { get; set; }
    }
}
