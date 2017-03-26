namespace DamSword.Common.Events
{
    public interface IEventPublisher : IService
    {
        void PublishEvent(object @event);
    }
}