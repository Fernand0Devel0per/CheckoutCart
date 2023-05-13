namespace CheckoutCart.Dtos.User
{
    public class UserUpdateRequest
    {
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
        public string Role { get; set; }
    }
}
