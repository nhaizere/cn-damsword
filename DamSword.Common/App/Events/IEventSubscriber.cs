namespace DamSword.Common.Events
{
    public interface IEventSubscriber<in T>
    {
        void HandleEvent(T @event);
    }
}
