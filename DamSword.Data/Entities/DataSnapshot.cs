using System;

namespace DamSword.Data.Entities
{
    public class DataSnapshot : EntityBase
    {
        public DateTime Date { get; set; }
        public long PersonId { get; set; }
        public long WebResourceId { get; set; }
        public int Type { get; set; }
        public byte[] Data { get; set; }
        
        public virtual Person Person { get; set; }
        public virtual WebResource WebResource { get; set; }
    }
}