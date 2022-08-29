using Accounting.Dtos;
using Accounting.Models;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Tests.Controllers
{
    public partial class AccountsControllerTests
    {
        [Fact]
        public void CreateAccount_ReturnsCreatedAtRouteResult()
        {
            // Arrange
            CreateSUT();

            // Act
            var accountCreateDto = new AccountCreateDto();
            var response = _sut.CreateAccount(accountCreateDto);

            // Assert
            Assert.IsType<CreatedAtRouteResult>(response.Result);
        }

        [Fact]
        public void CreateAccount_CallsRepositorySaveChanges()
        {
            // Arrange
            CreateSUT();

            // Act
            var accountCreateDto = new AccountCreateDto();
            var response = _sut.CreateAccount(accountCreateDto);

            // Assert
            _accountingRepositoryMock.Verify(r => r.SaveChanges(), Times.Once);
        }

        [Fact]
        public void CreateAccount_CallsRepositoryCreateAccount()
        {
            // Arrange
            CreateSUT();

            // Act
            var accountCreateDto = new AccountCreateDto();
            var response = _sut.CreateAccount(accountCreateDto);

            // Assert
            _accountingRepositoryMock.Verify(r => r.CreateAccount(It.IsAny<Account>()), Times.Once);
        }

        [Fact]
        public void CreateAccount_PublishesCreationToMessageBus()
        {
            // Arrange
            CreateSUT();

            // Act
            var accountCreateDto = new AccountCreateDto();
            var response = _sut.CreateAccount(accountCreateDto);

            // Assert
            _messageBusClient.Verify(b => b.PublishNewAccount(It.IsAny<AccountPublishedDto>()), Times.Once);
        }

    }
}