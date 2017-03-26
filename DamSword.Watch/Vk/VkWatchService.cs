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

        private const int PersonPerStackLimit = 20;
        private const string WebResourceName = "VKontakte";
        private const string WebResourceUrl = "https://vk.com";
        private const string WebResourceImageUrl = "/images/resources/vk-icon.png";

        public int MaxStackSize => PersonPerStackLimit;

        public void FetchOnline(IEnumerable<long> personIds)
        {
            if (personIds == null)
                throw new ArgumentNullException(nameof(personIds));
            if (personIds.Count() > MaxStackSize)
                throw new ArgumentException($"Maximum count is \"{MaxStackSize}\".", nameof(personIds));

            var webResourceId = GetOrFetchWebResourceId();
            var provider = GetOrFetchProvider(webResourceId);
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

        private long GetOrFetchWebResourceId()
        {
            var resourceId = WebResourceRepository.FirstOrDefault(r => r.Url == WebResourceUrl, r => r.Id);
            if (resourceId != 0)
                return resourceId;

            var resource = new WebResource
            {
                Name = WebResourceName,
                Url = WebResourceUrl,
                ImageUrl = WebResourceImageUrl
            };

            WebResourceRepository.Save(resource);
            UnitOfWork.Commit();

            return resource.Id;
        }

        private PersonMetaProvider GetOrFetchProvider(long webResourceId)
        {
            var providerName = typeof(VkWatchService).FullName;
            var provider = PersonMetaProviderRepository.FirstOrDefault(r => r.Name == providerName);
            if (provider == null)
            {
                provider = new PersonMetaProvider
                {
                    Name = providerName,
                    WebResourceId = webResourceId
                };

                PersonMetaProviderRepository.Save(provider);
                UnitOfWork.Commit();
            }

            return provider;
        }
    }
}