namespace DamSword.Data.Entities
{
    public class Watch : EntityBase
    {
        public long PersonId { get; set; }
        public long WebResourceId { get; set; }

        public virtual Person Person { get; set; }
        public virtual WebResource WebResource { get; set; }
    }
}