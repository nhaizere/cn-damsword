using System;
using System.Collections.Generic;
using DamSword.Common;

namespace DamSword.Watch
{
    public interface IWatchService : IService
    {
        IEnumerable<long> WebResourceIds { get; }
        string PersonMetaProviderUuid { get; }
        int MaxStackSize { get; }

        void EnsureRegistered();
        void FetchOnline(IEnumerable<long> personIds);
        void FetchMeta(IEnumerable<long> personIds, FetchType type, DateTime? until);
    }
}