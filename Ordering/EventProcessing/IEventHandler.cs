namespace Ordering.EventProcessing
{
    public interface IEventHandler
    {
        public void Handle(string message);
        public EventType EventType { get; }
    }
}