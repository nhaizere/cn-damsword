using DamSword.Common;
using DamSword.Data.Entities;

namespace DamSword.Web
{
    public class UserScope : ScopeBase<UserScope>
    {
        public User User { get; }

        public UserScope(User user)
        {
            User = user;
        }
    }
}