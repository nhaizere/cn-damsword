using DamSword.Common;

namespace DamSword.Web
{
    public class AppConfig : CommonAppConfig
    {
        public static string EntityContextConnectionStringName => "EntityContext";
    }
}