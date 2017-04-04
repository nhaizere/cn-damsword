using DamSword.Data.Entities;

namespace DamSword.Web.DTO.Objects
{
    public class User : ObjectBase
    {
        public string Alias { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int? HierarchyLevel { get; set; }
        public UserPermissions? Permissions { get; set; }
        public long? PersonId { get; set; }
    }
}