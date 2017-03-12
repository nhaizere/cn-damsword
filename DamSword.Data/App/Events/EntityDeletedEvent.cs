namespace DamSword.Data.Events
{
    public class EntityDeletedEvent<TEntity> : EntityEventBase<TEntity>
        where TEntity : IEntity
    {
    }
}
