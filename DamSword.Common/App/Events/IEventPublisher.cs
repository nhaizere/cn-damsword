namespace DamSword.Common.Events
{
    public interface IEventPublisher
    {
        void PublishEvent(object @event);
    }
}