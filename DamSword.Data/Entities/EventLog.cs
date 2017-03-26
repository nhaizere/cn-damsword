namespace DamSword.Data.Entities
{
    public class EventLog : EntityBase
    {
        public EventType Type { get; set; }
        public string DataJson { get; set; }
    }
}