using DamSword.Common;
using DamSword.Watch.Extensions;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DamSword.Watch.Vk.DTO
{
    public class User
    {
        [JsonProperty("uid")]
        public ulong Id { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("deactivated")]
        public string DeactivatedStatusName { get; set; }

        [JsonProperty("hidden")]
        public bool? IsVisibleOnlyForVkUsers { get; set; } // public requests only

        [JsonProperty("verified")]
        public bool? IsVerified { get; set; }

        [JsonProperty("blacklisted")]
        public bool? IsBlacklisted { get; set; }

        [JsonProperty("sex")]
        public int? SexId { get; set; }

        [JsonProperty("bdate")]
        public string BirthDate { get; set; }

        [JsonProperty("city")]
        public int? CityId { get; set; }

        [JsonProperty("country")]
        public int? CountryId { get; set; }

        [JsonProperty("home_town")]
        public string HomeTown { get; set; }

        [JsonProperty("photo_50")]
        public string Photo50Url { get; set; }

        [JsonProperty("photo_100")]
        public string Photo100Url { get; set; }

        [JsonProperty("photo_200")]
        public string Photo200Url { get; set; } // can be "false" if not exist

        [JsonProperty("photo_200_orig")]
        public string Photo200OrigUrl { get; set; }

        [JsonProperty("photo_400_orig")]
        public string Photo400OrigUrl { get; set; }

        [JsonProperty("photo_max")]
        public string PhotoMaxUrl { get; set; }

        [JsonProperty("photo_max_orig")]
        public string PhotoMaxOrigUrl { get; set; }

        [JsonProperty("online")]
        public bool? Online { get; set; }

        [JsonProperty("online_mobile")] // +online
        public bool? OnlineMobile { get; set; }

        [JsonProperty("online_app")] // +online
        public string OnlineApplication { get; set; }

        [JsonProperty("lists")]
        public IEnumerable<Friend> Friends { get; set; }

        [JsonProperty("domain")]
        public string Alias { get; set; }

        [JsonProperty("has_mobile")]
        public bool? HasMobile { get; set; }

        [JsonProperty("mobile_phone")] // +contacts
        public string MobilePhone { get; set; }

        [JsonProperty("home_phone")] // +contacts
        public string AlternativePhone { get; set; }

        [JsonProperty("site")]
        public string WebSite { get; set; }

        [JsonProperty("education")]
        public IEnumerable<Education> Educations { get; set; }

        [JsonProperty("universities")]
        public IEnumerable<University> Universities { get; set; }

        [JsonProperty("schools")]
        public IEnumerable<School> Schools { get; set; }

        [JsonProperty("activity")]
        public string Status { get; set; }

        [JsonProperty("last_seen")]
        public LastSeen LastSeen { get; set; }

        [JsonProperty("followers_count")]
        public int? FollowerCount { get; set; }

        [JsonProperty("common_count")]
        public int? MutualFriendCount { get; set; }

        [JsonProperty("counters")]
        public Counters Counters { get; set; } // null if blacklisted by user

        [JsonProperty("occupation")]
        public Occupation Occupation { get; set; }
        
        [JsonProperty("nickname")] // single user request only (from legacy API documantation)
        public string Nickname { get; set; }
        
        [JsonProperty("relatives")]
        public IEnumerable<Relativity> Relatives { get; set; }

        [JsonProperty("relation")]
        public int? RelationshipStatusId { get; set; }

        [JsonProperty("personal")]
        public Personal Personal { get; set; }

        [JsonProperty("skype")]
        public string Skype { get; set; } // +connections

        [JsonProperty("facebook")]
        public string Facebook { get; set; } // +connections

        [JsonProperty("twitter")]
        public string Twitter { get; set; } // +connections

        [JsonProperty("livejounal")]
        public string LiveJounal { get; set; } // +connections

        [JsonProperty("instagram")]
        public string Instagram { get; set; } // +connections
        
        [JsonProperty("exports")]
        public Exports Exports { get; set; } // standalone application only

        [JsonProperty("wall_comments")]
        public bool? WallComments { get; set; }
        
        [JsonProperty("activities")]
        public string Activities{ get; set; }

        [JsonProperty("interests")]
        public string Interests { get; set; }

        [JsonProperty("music")]
        public string Music { get; set; }

        [JsonProperty("movies")]
        public string Movies { get; set; }

        [JsonProperty("tv")]
        public string Tv { get; set; }

        [JsonProperty("books")]
        public string Books { get; set; }

        [JsonProperty("games")]
        public string Games { get; set; }
        
        [JsonProperty("about")]
        public string About { get; set; }

        [JsonProperty("quotes")]
        public string Quotes { get; set; }

        [JsonProperty("can_post")]
        public bool? CanPost { get; set; }

        [JsonProperty("can_see_all_posts")]
        public bool? CanSeeAllPosts { get; set; }

        [JsonProperty("can_see_audio")]
        public bool? CanSeeAudio { get; set; }

        [JsonProperty("can_write_private_message")]
        public bool? CanWritePrivateMessage { get; set; }

        [JsonProperty("timezone")]
        public int? Timezone { get; set; } // only while requesting current user info

        [JsonProperty("screen_name")]
        public bool? ScreenName { get; set; }
        
        #region Legacy

        [JsonProperty("photo")]
        public string Photo50Legacy { get; set; }

        [JsonProperty("photo_medium")]
        public string Photo100Legacy { get; set; }

        [JsonProperty("photo_big")]
        public string Photo200Legacy { get; set; }

        [JsonProperty("photo_rec")]
        public string Photo50RecLegacy { get; set; }

        [JsonProperty("rate")]
        public string Rating { get; set; }

        [JsonProperty("university")]
        public string UniversityCode { get; set; }

        [JsonProperty("university_name")]
        public string UniversityName { get; set; }

        [JsonProperty("Faculty")]
        public string FacultyCode { get; set; }

        [JsonProperty("faculty_name")]
        public string FacultyName { get; set; }

        [JsonProperty("graduation")]
        public string GraduationYear { get; set; }

        [JsonProperty("facebook_name")]
        public string FacebookName { get; set; } // +connections

        #endregion

        public string AccountReference => $"id{Id}";

        public DeactivatedStatus DeactivatedStatus => EnumExtensions.FromString(DeactivatedStatusName, DeactivatedStatus.Active);
        public SexType SexType => EnumExtensions.FromUnderlyingType(SexId, SexType.Unknown);
        public RelationshipStatus RelationshipStatus => EnumExtensions.FromUnderlyingType(RelationshipStatusId, RelationshipStatus.Unknown);

        public OnlineType OnlineType
        {
            get
            {
                if (Online != true)
                    return OnlineType.Offline;

                if (OnlineApplication.NonNullOrEmpty())
                    return OnlineType.Application;

                return OnlineMobile == true ? OnlineType.Mobile : OnlineType.Web;
            }
        }
    }
}