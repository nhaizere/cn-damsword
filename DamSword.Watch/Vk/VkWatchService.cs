using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DamSword.Data;
using DamSword.Data.Entities;
using DamSword.Data.Repositories;
using DamSword.Watch.Extensions;
using DamSword.Watch.Vk.Providers;

namespace DamSword.Watch.Vk
{
    public interface IVkWatchService : IWatchService
    {
    }

    public class VkWatchService : IVkWatchService
    {
        public IMetaDataSnapshotRepository MetaDataSnapshotRepository { get; set; }
        public IMetaAccountRepository MetaAccountRepository { get; set; }
        public IPersonMetaProviderRepository PersonMetaProviderRepository { get; set; }
        public IWebResourceRepository WebResourceRepository { get; set; }
        public IUnitOfWork UnitOfWork { get; set; }
        public IVkOnlineProvider VkOnlineProvider { get; set; }

        public const string WatchServiceUuid = "19178CEB-882F-4CCB-8EDD-EE0CF694E86F";
        private const int PersonPerStackLimit = 20;

        private const string WebResourceName = "VKontakte";
        private const string WebResourceUrl = "https://vk.com";
        private const string WebResourceImageUrl = "/images/resources/vk-icon.png";

        public string Uuid => WatchServiceUuid;
        public int MaxStackSize => PersonPerStackLimit;

        public void EnsureRegistered()
        {
            var webResourceId = WebResourceRepository.FirstOrDefault(r => r.Uuid == WebResourceList.Vk, r => r.Id);
            if (webResourceId == 0)
            {
                var resource = new WebResource
                {
                    Uuid = WebResourceList.Vk,
                    Name = WebResourceName,
                    Url = WebResourceUrl,
                    ImageUrl = WebResourceImageUrl
                };

                WebResourceRepository.Save(resource);
                UnitOfWork.Commit();

                webResourceId = resource.Id;
            }

            var isProviderRegistered = PersonMetaProviderRepository.Any(r => r.Uuid == Uuid);
            if (!isProviderRegistered)
            {
                PersonMetaProviderRepository.Save(new PersonMetaProvider
                {
                    Uuid = Uuid,
                    Name = typeof(VkWatchService).FullName,
                    WebResourceId = webResourceId
                });

                UnitOfWork.Commit();
            }
        }

        public void FetchOnline(IEnumerable<long> personIds)
        {
            if (personIds == null)
                throw new ArgumentNullException(nameof(personIds));
            if (personIds.Count() > MaxStackSize)
                throw new ArgumentException($"Maximum count is \"{MaxStackSize}\".", nameof(personIds));

            var webResourceId = WebResourceRepository.Single(r => r.Uuid == WebResourceList.Vk, r => r.Id);
            var provider = PersonMetaProviderRepository.Single(p => p.Uuid == Uuid);
            var personAccountDict = MetaAccountRepository.Select(a => a.WebResourceId == webResourceId && personIds.Contains(a.PersonId))
                .GroupBy(a => a.PersonId)
                .ToDictionary(g => g.Key, g => g.Select(a => a.AccountId));

            var allAccountIds = personAccountDict.SelectMany(pa => pa.Value).ToArray();
            var accountOnlineSnapshotDict = VkOnlineProvider.FetchOnlineSnapshots(allAccountIds).Result;
            foreach (var accountSnapshot in accountOnlineSnapshotDict)
            {
                var personId = personAccountDict.First(p => p.Value.Any(a => a == accountSnapshot.Key)).Key;
                var onlineSnapshots = accountOnlineSnapshotDict[accountSnapshot.Key];

                foreach (var onlineSnapshot in onlineSnapshots)
                {
                    var todayOnlineDataSnapshot = MetaDataSnapshotRepository.GetOrFetchDataSnapshot(provider.Id, personId, accountSnapshot.Key, onlineSnapshot.Time, (int)VkDataSnapshotType.Online);
                    var snapshots = todayOnlineDataSnapshot.GetSnapshots<VkOnlineSnapshot>().Append(onlineSnapshot).ToArray();

                    todayOnlineDataSnapshot.SetSnapshots(snapshots);
                    MetaDataSnapshotRepository.Save(todayOnlineDataSnapshot);
                }
            }

            UnitOfWork.Commit();
        }

        public void FetchMeta(IEnumerable<long> personIds, FetchType type, DateTime? until)
        {
            throw new NotImplementedException();
        }
    }
}