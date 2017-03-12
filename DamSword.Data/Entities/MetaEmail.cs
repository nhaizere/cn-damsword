using System.ComponentModel.DataAnnotations;
using DamSword.Common;

namespace DamSword.Data.Entities
{
    public class MetaEmail : MetaEntityBase
    {
        [Required, StringLength(EmailExtensions.MaxEmailAddressLength)]
        public string Address { get; set; }

        public long? WebResourceId { get; set; }

        public WebResource WebResource { get; set; }
    }
}