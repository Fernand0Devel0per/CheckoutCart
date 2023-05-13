using System.Text.RegularExpressions;

namespace CheckoutCart.Helpers.Extensions
{
    public static class  StringExtensions
    {
        public static bool IsValidPassword(this string password)
        {
            var passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
            return passwordRegex.IsMatch(password);
        }
    }
}
