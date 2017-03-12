namespace DamSword.Data.Events
{
    public interface IEntityEvent<TEntity>
    {
        TEntity Entity { get; set; }
    }

    public abstract class EntityEventBase<TEntity> : IEntityEvent<TEntity>
        where TEntity : IEntity
    {
        public TEntity Entity { get; set; }
    }
}
