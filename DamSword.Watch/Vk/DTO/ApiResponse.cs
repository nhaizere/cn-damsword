using Newtonsoft.Json;

namespace DamSword.Watch.Vk.DTO
{
    public class ApiResponse<TResponse>
    {
        [JsonProperty("response")]
        public TResponse Response { get; set; }
    }
}