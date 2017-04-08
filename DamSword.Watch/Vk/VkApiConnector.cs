using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DamSword.Common;
using DamSword.Watch.Vk.DTO;

namespace DamSword.Watch.Vk
{
    public interface IVkApiConnector : IService
    {
        Task<string> GetValidAccountId(string accountId);
        Task<Dictionary<string, IEnumerable<VkOnlineSnapshot>>> FetchOnlineSnapshots(IEnumerable<string> references);
    }

    public class VkApiConnector : IVkApiConnector
    {
        private const string GetValidAccountIdUrl = "https://api.vk.com/method/getProfiles?domains={0}&fields=id";
        private const string FetchOnlineUrl = "https://api.vk.com/method/getProfiles?domains={0}&fields=online,last_seen";
        private const string Version = "A1336E64-DDE9-426D-AA1D-063AA77367D3";

        public Task<string> GetValidAccountId(string accountId)
        {
            if (accountId == null)
                throw new ArgumentNullException(nameof(accountId));

            return Task.Run(async () =>
            {
                var url = string.Format(GetValidAccountIdUrl, accountId);
                var response = await ApiConnector.JsonRequest<ApiResponse<IEnumerable<User>>>(url, HttpMethod.Get);
                var result = response?.Response;
                var id = result?.FirstOrDefault()?.Id;

                return id != null ? $"id{id}" : null;
            });
        }

        public Task<Dictionary<string, IEnumerable<VkOnlineSnapshot>>> FetchOnlineSnapshots(IEnumerable<string> references)
        {
            if (references == null)
                throw new ArgumentNullException(nameof(references));

            return Task.Run(async () =>
            {
                var referenceList = references.Join(",");
                var url = string.Format(FetchOnlineUrl, referenceList);
                var response = await ApiConnector.JsonRequest<ApiResponse<IEnumerable<User>>>(url, HttpMethod.Get);
                var result = response?.Response;

                var snapshots = result
                    .GroupBy(p => p.AccountReference)
                    .ToDictionary(g => g.Key, g => (IEnumerable<VkOnlineSnapshot>)g.Select(p => new VkOnlineSnapshot
                    {
                        Version = Version,
                        Time = DateTime.Now,
                        Type = p.OnlineType,
                        ApplicationId = p.OnlineApplication,
                        LastActivity = p.LastSeen?.Time,
                        LastActivityPlatformId = p.LastSeen?.PlatformId,
                        LastActivityPlatformType = p.LastSeen?.PlatformType
                    }).ToArray());

                return snapshots;
            });
        }
    }
}