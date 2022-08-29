using Bogus;
using Ordering.Controllers;
using Ordering.Data;
using Ordering.Models;
using Ordering.Profiles;
using System.Diagnostics.CodeAnalysis;

namespace Ordering.Tests.Controllers
{
    public partial class OrdersControllerTests
    {
        private OrdersController? _sut;

        private Mock<IOrdersRepository> _ordersRepositoryMock = new Mock<IOrdersRepository>();

        private Faker<Order> _orderFaker;
        private Faker<Account> _accountFaker;

        private Mapper _mapper;

        public OrdersControllerTests()
        {
            _accountFaker = new Faker<Account>()
                .StrictMode(true)
                .RuleFor(a => a.ExternalId, f => f.Random.Number(32))
                .RuleFor(a => a.Orders, f => null!)
                .RuleFor(a => a.Id, f => f.Random.Number(32));

            _orderFaker = new Faker<Order>()
                .StrictMode(true)
                .RuleFor(o => o.Id, f => f.Random.Number(32))
                .RuleFor(o => o.Cost, f => f.Finance.Amount(0, 10, 2))
                .RuleFor(o => o.Account, f => _accountFaker.Generate())
                .RuleFor(o => o.AccountId, f => f.Random.Number(32));

            var autoMapperProfile = new OrdersProfile();
            var config = new MapperConfiguration(cfg => cfg.AddProfile(autoMapperProfile));
            _mapper = new Mapper(config);
        }

        [MemberNotNull(nameof(_sut))]
        private void CreateSUT()
        {
            _sut = new OrdersController(_ordersRepositoryMock.Object, _mapper);
        }

        private void SetupGetOrdersForAccount()
        {
            _ordersRepositoryMock.Setup(r => r.GetOrdersForAccount(It.IsAny<int>())).Returns(_orderFaker.GenerateBetween(0, 5));
        }
    }
}
