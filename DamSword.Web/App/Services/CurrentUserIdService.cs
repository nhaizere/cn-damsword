using DamSword.Data;
using DamSword.Data.Entities;

namespace DamSword.Web.Services
{
    public class CurrentUserProvider : ICurrentUserProvider
    {
        public User GetCurrentUser()
        {
            return UserScope.Current.User;
        }
    }
}