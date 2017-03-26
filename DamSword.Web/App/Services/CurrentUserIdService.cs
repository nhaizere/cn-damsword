using DamSword.Data;
using DamSword.Services;

namespace DamSword.Web.Services
{
    public class CurrentUserIdService : ICurrentUserIdService, IService
    {
        public long? GetCurrentUserId()
        {
            return UserScope.Current.User?.Id;
        }
    }
}