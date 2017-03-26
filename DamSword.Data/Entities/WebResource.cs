using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DamSword.Common;

namespace DamSword.Data.Entities
{
    public class WebResource : EntityBase
    {
        [Required, StringLength(255)]
        public string Name { get; set; }

        [StringLength(UriExtensions.MaxUrlLength)]
        public string Url { get; set; }

        [StringLength(UriExtensions.MaxUrlLength)]
        public string ImageUrl { get; set; }

        public virtual IEnumerable<MetaAccount> MetaAccounts { get; set; }
        public virtual IEnumerable<MetaDataSnapshot> MetaDataSnapshots { get; set; }
    }
}