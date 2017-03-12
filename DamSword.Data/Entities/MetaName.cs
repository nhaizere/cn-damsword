namespace DamSword.Data.Entities
{
    public enum MetaNameType
    {
        Unknown = -1,
        Real = 0,
        Virtual = 1
    }

    public enum MetaNameKind
    {
        First = 0,
        Last = 1,
        Middle = 2,
        Alias = 3
    }

    public class MetaName : MetaEntityBase
    {
        public MetaNameType NameType { get; set; }
        public MetaNameKind NameKind { get; set; }
        public string Name { get; set; }
        public long? WebResourceId { get; set; }

        public virtual WebResource WebResource { get; set; }
    }
}