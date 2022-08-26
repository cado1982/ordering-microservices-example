using Microsoft.AspNetCore.Mvc;
using Ordering.Dtos;
using Ordering.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Tests.Controllers
{
    public partial class OrdersControllerTests
    {
        [Fact]
        public void GetOrdersForAccount_WhenAccountDoesntExist_ReturnsNotFoundResult()
        {
            // Arrange
            CreateSUT();
            _ordersRepositoryMock.Setup(r => r.AccountExists(1)).Returns(false);

            // Act
            var response = _sut.GetOrdersForAccount(1);

            // Assert
            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public void GetOrdersForAccount_ReturnsCorrectOrders()
        {
            // Arrange
            var order = _orderFaker.Generate();
            CreateSUT();
            _ordersRepositoryMock.Setup(r => r.AccountExists(1)).Returns(true);
            _ordersRepositoryMock.Setup(r => r.GetOrdersForAccount(1)).Returns(new List<Order> { order });

            // Act
            var response = _sut.GetOrdersForAccount(1);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(response.Result);
            var returnValue = Assert.IsType<List<OrderReadDto>>(okObjectResult.Value);
            Assert.Collection(returnValue, o =>
            {
                Assert.Equal(order.AccountId, o.AccountId);
                Assert.Equal(order.Cost, o.Cost);
                Assert.Equal(order.Id, o.Id);
            });
        }

    }
}
