using System;
using DamSword.Common;

namespace DamSword.Watch
{
    public interface IWatchService : IService
    {
        void FetchOnline(long personId);
        void FetchDataSnapshot(long personId, DateTime? until);
    }
}