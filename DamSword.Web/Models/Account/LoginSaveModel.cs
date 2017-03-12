namespace DamSword.Web.Models.Account
{
    public class LoginSaveModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public bool Persistent { get; set; }
        public string ReturnUrl { get; set; }
    }
}