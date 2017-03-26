using System;
using System.Collections.Generic;
using DamSword.Common;

namespace DamSword.Watch
{
    public interface IWatchService : IService
    {
        int MaxStackSize { get; }
        void FetchOnline(IEnumerable<long> personIds);
        void FetchMeta(IEnumerable<long> personIds, FetchType type, DateTime? until);
    }
}