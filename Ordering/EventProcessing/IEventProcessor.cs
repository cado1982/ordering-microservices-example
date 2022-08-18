namespace Ordering.EventProcessing
{
    public interface IEventProcessor
    {
        void ProcessEvent(string message);
    }
}