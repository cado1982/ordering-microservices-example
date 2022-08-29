using Ordering.Models;

namespace Ordering.Data
{
    public class OrdersRepository : IOrdersRepository
    {
        private readonly AppDbContext _context;

        public OrdersRepository(AppDbContext context)
        {
            _context = context;
        }

        public bool AccountExists(int accountId)
        {
            return _context.Accounts.Any(a => a.Id == accountId);
        }

        public void CreateAccount(Account account)
        {
            ArgumentNullException.ThrowIfNull(account);

            _context.Accounts.Add(account);

        }

        public void CreateOrder(int accountId, Order order)
        {
            ArgumentNullException.ThrowIfNull(order);

            order.AccountId = accountId;
            _context.Orders.Add(order);
        }

        public bool ExternalAccountExists(int externalAccountId)
        {
            return _context.Accounts.Any(a => a.ExternalId == externalAccountId);
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            return _context.Accounts.ToList();
        }

        public Order? GetOrder(int orderId, int accountId)
        {
            return _context.Orders
                .Where(o => o.AccountId == accountId && o.Id == orderId)
                .FirstOrDefault();
        }

        public IEnumerable<Order> GetOrdersForAccount(int accountId)
        {
            return _context.Orders.Where(o => o.AccountId == accountId);
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() >= 0;
        }
    }
}