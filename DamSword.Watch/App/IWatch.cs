using System;
using System.Collections.Generic;
using DamSword.Common;

namespace DamSword.Watch
{
    public interface IWatch : IService
    {
        long WebResourceId { get; }
        string PersonMetaProviderUuid { get; }
        int MaxStackSize { get; }

        void EnsureRegistered();
        string GetValidAccountId(string accountId);
        void FetchOnline(IEnumerable<long> personIds);
        void FetchMeta(IEnumerable<long> personIds, FetchType type, DateTime? until);
    }
}