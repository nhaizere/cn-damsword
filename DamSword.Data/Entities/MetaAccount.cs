using System.ComponentModel.DataAnnotations;

namespace DamSword.Data.Entities
{
    public class MetaAccount : MetaEntityBase
    {
        [Required]
        public string AccountId { get; set; }

        public long WebResourceId { get; set; }

        public virtual WebResource WebResource { get; set; }
    }
}