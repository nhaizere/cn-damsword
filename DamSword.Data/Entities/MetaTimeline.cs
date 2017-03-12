namespace DamSword.Data.Entities
{
    public class MetaTimeline : MetaEntityBase
    {
        public long WebResourceId { get; set; }
        public byte[] Data { get; set; }

        public virtual WebResource WebResource { get; set; }
    }
}