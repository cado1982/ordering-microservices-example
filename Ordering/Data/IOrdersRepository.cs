using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ordering.Models;

namespace Ordering.Data
{
    public interface IOrdersRepository
    {
        bool SaveChanges();

        IEnumerable<Account> GetAllAccounts();
        bool AccountExists(int accountId);
        bool ExternalAccountExists(int externalAccountId);
        void CreateAccount(Account account);

        IEnumerable<Order> GetOrdersForAccount(int accountId);
        Order GetOrder(int orderId, int accountId);
        void CreateOrder(int accountId, Order order);
    }
}