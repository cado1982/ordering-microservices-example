using Accounting.AsyncDataServices;
using Accounting.Controllers;
using Accounting.Data;
using Accounting.Models;
using Accounting.Profiles;
using Bogus;
using System.Diagnostics.CodeAnalysis;

namespace Accounting.Tests.Controllers;

public partial class AccountsControllerTests
{
    private AccountsController? _sut;

    private Mock<IAccountingRepository> _accountingRepositoryMock = new Mock<IAccountingRepository>();
    private Mock<IMessageBusClient> _messageBusClient = new Mock<IMessageBusClient>();

    private Faker<Account> _accountFaker;
    private Mapper _mapper;

    public AccountsControllerTests()
    {
        _accountFaker = new Faker<Account>()
            .StrictMode(true)
            .RuleFor(a => a.Id, f => f.Random.Number(32));

        var autoMapperProfile = new AccountsProfile();
        var config = new MapperConfiguration(cfg => cfg.AddProfile(autoMapperProfile));
        _mapper = new Mapper(config);
    }

    [MemberNotNull(nameof(_sut))]
    private void CreateSUT()
    {
        _sut = new AccountsController(
            _accountingRepositoryMock.Object,
            _mapper,
            _messageBusClient.Object);
    }

    private void SetupGetAccounts(int numberOfAccountsToReturn)
    {
        _accountingRepositoryMock.Setup(r => r.GetAccounts()).Returns(_accountFaker.Generate(5));
    }

    private void SetupGetAccountById()
    {
        _accountingRepositoryMock.Setup(r => r.GetAccountById(It.IsAny<int>()))
                                 .Returns(_accountFaker.Generate());
    }
}