using System.ComponentModel.DataAnnotations;

namespace DamSword.Data.Entities
{
    public class User : EntityBase
    {
        [Required]
        public string Alias { get; set; }

        [Required]
        public string Login { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public UserPermissions Permissions { get; set; }

        public long? PersonId { get; set; }
        public virtual Person Person { get; set; }
    }
}