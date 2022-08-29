using Microsoft.AspNetCore.Mvc;
using Ordering.Dtos;
using Ordering.Models;

namespace Ordering.Tests.Controllers
{
    public partial class OrdersControllerTests
    {
        [Fact]
        public void CreateOrder_WhenAccountDoesntExist_ReturnsNotFoundResult()
        {
            // Arrange
            CreateSUT();

            int accountId = 1;
            _ordersRepositoryMock.Setup(r => r.AccountExists(accountId)).Returns(false);

            // Act
            var response = _sut.CreateOrder(accountId, null!);

            // Assert
            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public void CreateOrder_SavesOrderInDatabase()
        {
            // Arrange
            CreateSUT();

            int accountId = 1;
            int orderId = 1;

            _ordersRepositoryMock.Setup(r => r.AccountExists(accountId)).Returns(true);
            _ordersRepositoryMock.Setup(r => r.GetOrder(accountId, orderId)).Returns((Order)null!);
            var orderCost = 5.00m;
            var order = new OrderCreateDto
            {
                Cost = orderCost
            };

            // Act
            var response = _sut.CreateOrder(1, order);

            // Assert
            _ordersRepositoryMock.Verify(r => r.CreateOrder(accountId, It.Is<Order>(o => o.Cost == orderCost)), Times.Once);
            _ordersRepositoryMock.Verify(r => r.SaveChanges(), Times.Once);
        }

        [Fact]
        public void CreateOrder_ReturnsCorrectOrder()
        {
            // Arrange
            CreateSUT();

            int accountId = 1;
            int orderId = 2;

            _ordersRepositoryMock.Setup(r => r.AccountExists(accountId)).Returns(true);

            var orderCost = 5.00m;
            var order = new OrderCreateDto
            {
                Cost = orderCost
            };
            _ordersRepositoryMock.Setup(r => r.CreateOrder(accountId, It.IsAny<Order>())).Callback<int, Order>((aId, o) => o.Id = orderId);

            // Act
            var response = _sut.CreateOrder(1, order);

            // Assert
            var createdAtRouteResult = Assert.IsType<CreatedAtRouteResult>(response.Result);
            var returnValue = Assert.IsType<OrderReadDto>(createdAtRouteResult.Value);

            Assert.Equal("GetOrderForAccount", createdAtRouteResult.RouteName);
            Assert.Collection(createdAtRouteResult.RouteValues, v =>
            {
                Assert.Equal(nameof(accountId), v.Key);
                Assert.Equal(accountId, v.Value);

            }, v =>
            {
                Assert.Equal(nameof(orderId), v.Key);
                Assert.Equal(orderId, v.Value);
            });

            Assert.Equal(accountId, returnValue.AccountId);
            Assert.Equal(orderCost, returnValue.Cost);
            Assert.Equal(orderId, returnValue.Id);
        }
    }
}
