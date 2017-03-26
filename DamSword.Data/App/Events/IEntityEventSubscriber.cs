using DamSword.Common.Events;

namespace DamSword.Data.Events
{
    public interface IEntityEventSubscriber<TEntity> : IEventSubscriber<EntityEventBase<TEntity>>
        where TEntity : IEntity
    {
    }
}
