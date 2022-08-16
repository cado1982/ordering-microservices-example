using Ordering.Models;

namespace Ordering.SyncDataService.Grpc
{
    public interface IAccountsDataClient
    {
        IEnumerable<Account> GetAllAccounts();
    }
}