using DamSword.Common;

namespace DamSword.Services
{
    public interface IWebResourceAccountIdValidator : IService
    {
        bool IsValidAccountId(long webResourceId, string accountId);
        string GetValidAccountId(long webResourceId, string accountId);
    }
}