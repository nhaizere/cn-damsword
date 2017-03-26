using DamSword.Data;

namespace DamSword.Web.Services
{
    public class CurrentUserIdService : ICurrentUserIdService
    {
        public long? GetCurrentUserId()
        {
            return UserScope.Current.User?.Id;
        }
    }
}