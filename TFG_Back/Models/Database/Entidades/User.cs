namespace TFG_Back.Models.Database.Entidades
{
    public class User
    {
        public int UserId { get; set; }
        public string UserProfilePhoto { get; set; }
        public string UserNickname{ get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
        public string UserConfirmPassword { get; set; }
        public string Role { get; set; } = "usuario";
    }
}
