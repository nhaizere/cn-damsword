using DamSword.Data.Entities;

namespace DamSword.Web
{
    public static class Permissions
    {
        public static bool Has(UserPermissions permissions)
        {
            var currentPermissions = UserScope.Current.User?.Permissions ?? UserPermissions.None;
            return (currentPermissions & permissions) == permissions;
        }

        public static bool IsOwner()
        {
            return Has(UserPermissions.Owner);
        }
    }
}