using Newtonsoft.Json;

namespace DamSword.Watch.Vk.DTO
{
    public class Education
    {
        [JsonProperty("university")]
        public int? UniversityId { get; set; }

        [JsonProperty("university_name")]
        public string UniversityName { get; set; }

        [JsonProperty("faculty")]
        public int? FacultyId { get; set; }

        [JsonProperty("faculty_name")]
        public string FacultyName { get; set; }

        [JsonProperty("graduation")]
        public int? GraduationYear { get; set; }
    }
}