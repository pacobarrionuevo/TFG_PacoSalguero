namespace TFG_Back.Models.DTO
{
    public class UserSignUpDTO
    {
        public IFormFile UserProfilePhoto { get; set; }
        public string UserNickname { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
        public string UserConfirmPassword { get; set; }
    }
}
