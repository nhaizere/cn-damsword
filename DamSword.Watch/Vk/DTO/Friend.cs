using Newtonsoft.Json;

namespace DamSword.Watch.Vk.DTO
{
    public class Friend
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}