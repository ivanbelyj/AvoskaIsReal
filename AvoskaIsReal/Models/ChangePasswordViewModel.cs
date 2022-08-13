namespace AvoskaIsReal.Models
{
    public class ChangePasswordViewModel
    {
        // Старый пароль может не требоваться, если пароль меняет админ или
        // владелец.
        public string? OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
        public string UserId { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
