using DamSword.Common;

namespace DamSword.Data
{
    public interface ICurrentUserIdService : IService
    {
        long? GetCurrentUserId();
    }
}