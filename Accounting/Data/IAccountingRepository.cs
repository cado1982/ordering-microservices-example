using Accounting.Models;

namespace Accounting.Data
{
    public interface IAccountingRepository
    {
        bool SaveChanges();
        void CreateAccount(Account account);
        IEnumerable<Account> GetAccounts();
        Account? GetAccountById(int id);
    }
}