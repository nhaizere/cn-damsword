using Newtonsoft.Json;

namespace DamSword.Watch.Vk.DTO
{
    public class School
    {
        [JsonProperty("country")]
        public int? CountryId { get; set; }

        [JsonProperty("city")]
        public int? CityId { get; set; }

        [JsonProperty("id")]
        public int? SchoolId { get; set; }

        [JsonProperty("name")]
        public string SchoolName { get; set; }
        
        [JsonProperty("class")]
        public string ClassName { get; set; }

        [JsonProperty("speciality ")]
        public string Speciality { get; set; }

        [JsonProperty("year_from")]
        public int? FromYear { get; set; }

        [JsonProperty("year_to")]
        public int? ToYear { get; set; }

        [JsonProperty("year_graduated")]
        public int? GraduationYear { get; set; }

        [JsonProperty("type")]
        public int? TypeId { get; set; }

        [JsonProperty("type_str")]
        public string TypeName { get; set; }
    }
}