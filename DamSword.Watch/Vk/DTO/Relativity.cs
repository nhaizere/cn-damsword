using DamSword.Watch.Extensions;
using Newtonsoft.Json;

namespace DamSword.Watch.Vk.DTO
{
    public class Relativity
    {
        [JsonProperty("uid")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string TypeName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        public RelativityType Type => EnumExtensions.FromString(TypeName, RelativityType.Unknown);
    }
}