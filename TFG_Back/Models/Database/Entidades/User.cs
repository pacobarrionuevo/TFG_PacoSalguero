using TFG_Back.Models.Database.Repositorios;

namespace TFG_Back.Models.Database.Entidades
{
    public class User
    {
        public int UserId { get; set; }
        public string UserProfilePhoto { get; set; }
        public string UserNickname { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
        public string UserConfirmPassword { get; set; }
        public string Role { get; set; } = "user";
        public List<UserHasFriendship> UserFriendship { get; set; }

        public bool IsOnline { get; set; } = false;
        public DateTime? LastSeen { get; set; }
    }
}
