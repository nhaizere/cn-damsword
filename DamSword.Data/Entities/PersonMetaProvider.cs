using System.ComponentModel.DataAnnotations;

namespace DamSword.Data.Entities
{
    public class PersonMetaProvider : EntityBase
    {
        [Required]
        public string Uuid { get; set; }

        [Required, StringLength(255)]
        public string Name { get; set; }

        public long WebResourceId { get; set; }

        public virtual WebResource WebResource { get; set; }
    }
}