using DamSword.Data.Entities;

namespace DamSword.Web
{
    public static class CurrentUser
    {
        public static User User => UserScope.Current.User ?? new User
        {
            Alias = "Anonymous",
            HierarchyLevel = int.MaxValue,
            Permissions = UserPermissions.None
        };

        public static bool IsAnonymous => User.Id == 0;
        public static long Id => User.Id;
        public static int Hierarchy => User.HierarchyLevel;
    }
}