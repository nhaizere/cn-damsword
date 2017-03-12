namespace DamSword.Common.Events
{
    public interface IOrderedEventSubscriber
    {
        int? HandleEventOrder { get; }
    }

    public interface IOrderedEventSubscriber<in T> : IEventSubscriber<T>, IOrderedEventSubscriber
    {
    }
}