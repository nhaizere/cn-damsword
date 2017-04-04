using DamSword.Common;
using DamSword.Data.Entities;

namespace DamSword.Data
{
    public interface ICurrentUserProvider : IService
    {
        User GetCurrentUser();
    }
}