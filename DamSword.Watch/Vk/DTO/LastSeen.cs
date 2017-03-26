using System;
using DamSword.Common;
using DamSword.Watch.Extensions;
using Newtonsoft.Json;

namespace DamSword.Watch.Vk.DTO
{
    public class LastSeen
    {
        [JsonProperty("time")]
        public long UnixTimeStamp { get; set; }

        [JsonProperty("platform")]
        public int? PlatformId { get; set; }

        public DateTime Time => DateTimeExtensions.FromUnixTimeStamp(UnixTimeStamp);
        public PlatformType PlatformType => EnumExtensions.FromUnderlyingType(PlatformId, PlatformType.Unknown);
    }
}