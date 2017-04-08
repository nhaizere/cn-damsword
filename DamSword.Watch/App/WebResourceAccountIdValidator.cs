using System.Collections.Generic;
using System.Linq;
using DamSword.Services;

namespace DamSword.Watch
{
    public class WebResourceAccountIdValidator : IWebResourceAccountIdValidator
    {
        public IEnumerable<IWatch> WatchServices { get; set; }

        public bool IsValidAccountId(long webResourceId, string accountId)
        {
            return GetValidAccountId(webResourceId, accountId) == accountId;
        }

        public string GetValidAccountId(long webResourceId, string accountId)
        {
            var watchService = WatchServices.FirstOrDefault(s => s.WebResourceId == webResourceId);
            return watchService?.GetValidAccountId(accountId);
        }
    }
}