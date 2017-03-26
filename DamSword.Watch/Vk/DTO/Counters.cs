using Newtonsoft.Json;

namespace DamSword.Watch.Vk.DTO
{
    public class Counters
    {
        [JsonProperty("albums")]
        public int? Albums { get; set; }

        [JsonProperty("videos")]
        public int? Videos { get; set; }

        [JsonProperty("audios")]
        public int? Musics { get; set; }

        [JsonProperty("notes")]
        public int? Notes { get; set; }

        [JsonProperty("friends")]
        public int? Friends { get; set; }

        [JsonProperty("online_friends")]
        public int? OnlineFriends { get; set; }

        [JsonProperty("mutual_friends")]
        public int? MutualFriends { get; set; }

        [JsonProperty("groups")]
        public int? Groups { get; set; }

        [JsonProperty("followers")]
        public int? Followers { get; set; }

        [JsonProperty("subscriptions")]
        public int? Subscriptions { get; set; }

        [JsonProperty("user_photos")]
        public int? RelatedPhotos { get; set; }

        [JsonProperty("user_videos")]
        public int? RelatedVideos { get; set; }
    }
}