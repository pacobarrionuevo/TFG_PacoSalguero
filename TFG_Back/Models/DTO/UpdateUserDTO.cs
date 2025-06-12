namespace TFG_Back.Models.DTO
{
    // DTO para que un usuario actualice su propia información.
    public class UpdateUserDTO
    {
        public string UserNickname { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
        public string UserConfirmPassword { get; set; }
    }
}