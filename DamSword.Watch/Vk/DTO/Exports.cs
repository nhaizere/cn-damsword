using Newtonsoft.Json;

namespace DamSword.Watch.Vk.DTO
{
    public class Exports
    {
        [JsonProperty("facebook")]
        public string Facebook { get; set; }
        
        [JsonProperty("twitter")]
        public string Twitter { get; set; }

        [JsonProperty("livejounal")]
        public string LiveJounal { get; set; }

        [JsonProperty("instagram")]
        public string Instagram { get; set; }
    }
}