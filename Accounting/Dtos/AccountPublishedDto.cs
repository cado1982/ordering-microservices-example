using Accounting.EventProcessing;

namespace Accounting.Dtos
{
    public class AccountPublishedDto
    {
        public int Id { get; set; }
        public EventType EventType { get; set; }
    }
}