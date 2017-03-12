namespace DamSword.Data.Entities
{
    public enum MetaConnectionType
    {
        SamePerson = 0,
        Contact = 1,
        Friend = 2,
        Family = 3
    }

    public class MetaConnection : MetaEntityBase
    {
        public MetaConnectionType ConnectionType { get; set; }

        public long ConnectedPersonId { get; set; }
        public long? WebResourceId { get; set; }

        public Person ConnectedPerson { get; set; }
        public WebResource WebResource { get; set; }
    }
}