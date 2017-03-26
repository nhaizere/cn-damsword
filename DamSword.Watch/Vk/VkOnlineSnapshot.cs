using System;
using DamSword.Watch.Vk.DTO;

namespace DamSword.Watch.Vk
{
    public class VkOnlineSnapshot
    {
        public DateTime Time { get; set; }
        public OnlineType Type { get; set; }
        public string ApplicationId { get; set; }
        public DateTime? LastActivity { get; set; }
        public int? LastActivityPlatformId { get; set; }
        public PlatformType? LastActivityPlatformType { get; set; }
    }
}