using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DamSword.Common;
using DamSword.Watch.Vk.DTO;

namespace DamSword.Watch.Vk.Providers
{
    public interface IVkOnlineProvider : IService
    {
        Task<Dictionary<string, IEnumerable<VkOnlineSnapshot>>> FetchOnlineSnapshots(IEnumerable<string> references);
    }

    public class VkOnlineProvider : IVkOnlineProvider
    {
        private const string FetchOnlineUrl = "https://api.vk.com/method/getProfiles?domains={0}&fields=online,last_seen";

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