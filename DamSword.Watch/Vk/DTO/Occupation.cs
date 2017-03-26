using Newtonsoft.Json;

namespace DamSword.Watch.Vk.DTO
{
    public class Occupation
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}