namespace TFG_Back.Models.DTO
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public string UserProfilePhoto { get; set; }
        public string UserNickname { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
        public string UserConfirmPassword { get; set; }
        public string Role { get; set; } = "usuario";
    }
}
