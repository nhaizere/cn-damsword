namespace DamSword.Data.Entities
{
    public class User : EntityBase
    {
        public string Alias { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        
        public long? PersonId { get; set; }
        public virtual Person Person { get; set; }
    }
}