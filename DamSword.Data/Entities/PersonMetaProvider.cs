namespace DamSword.Data.Entities
{
    public class PersonMetaProvider : EntityBase
    {
        public string Name { get; set; }
        public long WebResourceId { get; set; }

        public virtual WebResource WebResource { get; set; }
    }
}