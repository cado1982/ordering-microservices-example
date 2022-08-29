using Microsoft.AspNetCore.Mvc;

namespace Accounting.Tests.Controllers
{
    public partial class AccountsControllerTests
    {
        [Fact]
        public void GetAccountById_ReturnsOkResult()
        {
            // Arrange
            CreateSUT();
            SetupGetAccountById();

            // Act
            var response = _sut.GetAccountById(1);

            // Assert
            Assert.IsType<OkObjectResult>(response.Result);
        }

        [Fact]
        public void GetAccountById_ReturnsAccount()
        {
            // Arrange
            CreateSUT();
            SetupGetAccountById();

            // Act
            var response = _sut.GetAccountById(1);
            var result = response.Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result!.Value);
        }

        [Fact]
        public void GetAccountById_WhenAccountDoesntExist_ReturnsNotFound()
        {
            // Arrange
            CreateSUT();

            // Act
            var response = _sut.GetAccountById(1);

            // Assert
            Assert.IsType<NotFoundResult>(response.Result);
        }
    }
}