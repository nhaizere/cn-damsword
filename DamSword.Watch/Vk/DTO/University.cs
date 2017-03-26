using Newtonsoft.Json;

namespace DamSword.Watch.Vk.DTO
{
    public class University
    {
        [JsonProperty("country")]
        public int? CountryId { get; set; }

        [JsonProperty("city")]
        public int? CityId { get; set; }

        [JsonProperty("id")]
        public int? UniversityId { get; set; }

        [JsonProperty("name")]
        public string UniversityName { get; set; }

        [JsonProperty("faculty")]
        public int? FacultyId { get; set; }

        [JsonProperty("faculty_name")]
        public string FacultyName { get; set; }

        [JsonProperty("chair")]
        public int? ChairId { get; set; }

        [JsonProperty("chair_name")]
        public string ChairName { get; set; }

        [JsonProperty("graduation")]
        public int? GraduationYear { get; set; }
    }
}