using SimpleCrypto;

namespace DamSword.Web
{
    public static class PasswordUtils
    {
        private const string Salt = "100000./zgfg8PwH08R11WoU1xMKHiLE9AVcZeQkSimax/UAdqvhw==";

        public static string CreateHash(string password)
        {
            var cryptoService = new PBKDF2();
            return cryptoService.Compute(password, Salt);
        }
    }
}