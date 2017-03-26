using System.Collections.Generic;

namespace DamSword.Data.Entities
{
    public class Person : EntityBase
    {
        public virtual ICollection<MetaAccount> Accounts { get; set; }
        public virtual ICollection<MetaConnection> Connections { get; set; }
        public virtual ICollection<MetaConnection> ConnectedTo { get; set; }
        public virtual ICollection<MetaEmail> Emails { get; set; }
        public virtual ICollection<MetaName> Names { get; set; }
    }
}