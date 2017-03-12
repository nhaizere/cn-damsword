using DamSword.Common.Events;

namespace DamSword.Data.Events
{
    public interface IEntityModifiedEventSubscriber<TEntity> : IEventSubscriber<EntityModifiedEvent<TEntity>>
        where TEntity : IEntity
    {
    }
}
