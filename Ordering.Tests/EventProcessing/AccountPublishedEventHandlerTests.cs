using Ordering.Data;
using Ordering.EventProcessing;
using Ordering.Models;
using Ordering.Profiles;
using System.Diagnostics.CodeAnalysis;

namespace Ordering.Tests.EventProcessing
{
    public class AccountPublishedEventHandlerTests
    {
        private AccountPublishedEventHandler? _sut;

        private Mock<IOrdersRepository> _ordersRepositoryMock = new Mock<IOrdersRepository>();
        private IMapper _mapper;

        public AccountPublishedEventHandlerTests()
        {
            var autoMapperProfile = new OrdersProfile();
            var config = new MapperConfiguration(cfg => cfg.AddProfile(autoMapperProfile));
            _mapper = new Mapper(config);
        }

        [MemberNotNull(nameof(_sut))]
        private void CreateSUT()
        {
            _sut = new AccountPublishedEventHandler(_mapper, _ordersRepositoryMock.Object);
        }

        [Fact]
        public void Handle_WhenAccountDoesntExist_AddsAccountToRepository()
        {
            // Arrange
            CreateSUT();
            var account = new Account { Id = 1 };

            // Act
            var message = $"{{ \"Id\": { account.Id }}}";
            _sut.Handle(message);

            // Assert
            _ordersRepositoryMock.Verify(r => r.CreateAccount(It.Is<Account>(a => a.ExternalId == account.Id)), Times.Once);
            _ordersRepositoryMock.Verify(r => r.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Handle_WhenAccountAlreadyExists_DoesntAddAccountToRepository()
        {
            // Arrange
            CreateSUT();
            var account = new Account { Id = 1 };
            _ordersRepositoryMock.Setup(r => r.ExternalAccountExists(account.Id)).Returns(true);

            // Act
            var message = $"{{ \"Id\": { account.Id }}}";
            _sut.Handle(message);

            // Assert
            _ordersRepositoryMock.Verify(r => r.CreateAccount(It.IsAny<Account>()), Times.Never);
            _ordersRepositoryMock.Verify(r => r.SaveChanges(), Times.Never);
        }
    }
}
