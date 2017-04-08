namespace DamSword.Data.Entities
{
    public enum MetaType
    {
        Unknown = -1,
        Public = 0,
        Inferred = 1,
        Created = 10
    }

    public class MetaEntityBase : EntityBase
    {
        public long PersonId { get; set; }
        public long? ProviderId { get; set; }
        public MetaType Type { get; set; }

        public virtual Person Person { get; set; }
        public virtual PersonMetaProvider Provider { get; set; }
    }
}