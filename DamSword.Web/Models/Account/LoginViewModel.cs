namespace DamSword.Web.Models.Account
{
    public class LoginViewModel
    {
        public bool InvalidCredentials { get; set; }
        public string ReturnUrl { get; set; }
    }
}