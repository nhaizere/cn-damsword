namespace DamSword.Data
{
    public static class EntityExtensions
    {
        public static bool IsNew(this IEntity self)
        {
            return self.Id == 0;
        }
    }
}
