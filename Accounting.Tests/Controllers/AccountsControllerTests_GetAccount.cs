using Accounting.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Tests.Controllers
{
    public partial class AccountsControllerTests
    {
        [Fact]
        public void GetAccounts_ReturnsOkResult()
        {
            // Arrange
            CreateSUT();

            // Act
            var response = _sut.GetAccounts();

            // Assert
            Assert.IsType<OkObjectResult>(response.Result);
        }

        [Fact]
        public void GetAccounts_ReturnsCorrectAccounts()
        {
            // Arrange
            CreateSUT();
            SetupGetAccounts(5);

            // Act
            var response = _sut.GetAccounts();

            // Assert
            var list = response.Result as OkObjectResult;
            Assert.IsType<List<AccountReadDto>>(list!.Value);

            var accounts = list.Value as List<AccountReadDto>;
            Assert.Equal(5, accounts!.Count);
        }
    }
}