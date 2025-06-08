namespace TFG_Back.Models.Database.Entidades
{
    public class FriendShip
    {
        public int FriendShipId { get; set; }
        public bool IsAccepted { get; set; } = false;
        public List<UserHasFriendship> UserFriendship { get; set; }
    }
}
