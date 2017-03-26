namespace DamSword.Common.Events
{
    public interface IEventSubscriber<in T> : IService
    {
        void HandleEvent(T @event);
    }
}
