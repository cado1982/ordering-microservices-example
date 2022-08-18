using Accounting.Dtos;

namespace Accounting.AsyncDataServices
{
    public interface IMessageBusClient
    {
        void PublishNewAccount(AccountPublishedDto accountPublishedDto);
    }
}