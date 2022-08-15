using Accounting.Models;

namespace Accounting.Data
{
    public class AccountingRepository : IAccountingRepository
    {
        private readonly AppDbContext _context;

        public AccountingRepository(AppDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            _context = context;
        }

        public IEnumerable<Account> GetAccounts()
        {
            return _context.Accounts.ToList();
        }

        public void CreateAccount(Account account)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));

            _context.Accounts.Add(account);
        }

        public Account? GetAccountById(int id)
        {
            return _context.Accounts.FirstOrDefault(a => a.Id == id);
        }        

        public bool SaveChanges()
        {
            return _context.SaveChanges() >= 0;
        }

        
    }
}