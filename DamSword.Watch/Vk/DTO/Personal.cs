using DamSword.Watch.Extensions;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DamSword.Watch.Vk.DTO
{
    public class Personal
    {
        [JsonProperty("political")]
        public int? PoliticalViewsId { get; set; }

        [JsonProperty("langs")]
        public IEnumerable<string> Languages { get; set; }

        [JsonProperty("religion")]
        public string WorldView { get; set; }

        [JsonProperty("inspired_by")]
        public string InspiredBy { get; set; }

        [JsonProperty("people_main")]
        public int? ImportantInOthersId { get; set; }

        [JsonProperty("life_main")]
        public int? PersonalPriorityId { get; set; }

        [JsonProperty("smoking")]
        public int? AttitudeTowardsSmokingId { get; set; }

        [JsonProperty("alcohol")]
        public int? AttitudeTowardsAlcoholId { get; set; }

        public PoliticalViewsType PoliticalViewsType => EnumExtensions.FromUnderlyingType(PoliticalViewsId, PoliticalViewsType.Unknown);
        public ImportantInOthersType ImportantInOthersType => EnumExtensions.FromUnderlyingType(ImportantInOthersId, ImportantInOthersType.Unknown);
        public PersonalPriorityType PersonalPriorityType => EnumExtensions.FromUnderlyingType(PersonalPriorityId, PersonalPriorityType.Unknown);
        public PersonalAttitudeType AttitudeTowardsSmokingType => EnumExtensions.FromUnderlyingType(AttitudeTowardsSmokingId, PersonalAttitudeType.Unknown);
        public PersonalAttitudeType AttitudeTowardsAlcoholType => EnumExtensions.FromUnderlyingType(AttitudeTowardsAlcoholId, PersonalAttitudeType.Unknown);
    }
}