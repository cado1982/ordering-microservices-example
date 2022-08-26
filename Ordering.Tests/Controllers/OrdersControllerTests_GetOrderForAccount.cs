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
        public void GetOrderForAccount_WhenAccountDoesntExist_ReturnsNotFoundResult()
        {
            // Arrange
            CreateSUT();
            _ordersRepositoryMock.Setup(r => r.AccountExists(1)).Returns(false);

            // Act
            var response = _sut.GetOrderForAccount(1, 1);

            // Assert
            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public void GetOrderForAccount_WhenOrderDoesntExist_ReturnsNotFoundResult()
        {
            // Arrange
            CreateSUT();
            _ordersRepositoryMock.Setup(r => r.AccountExists(1)).Returns(true);
            _ordersRepositoryMock.Setup(r => r.GetOrder(1, 1)).Returns((Order)null!);

            // Act
            var response = _sut.GetOrderForAccount(1, 1);

            // Assert
            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public void GetOrderForAccount_ReturnsCorrectOrder()
        {
            // Arrange
            int accountId = 1;
            int orderId = 1;

            var order = _orderFaker.Generate();
            CreateSUT();
            _ordersRepositoryMock.Setup(r => r.AccountExists(accountId)).Returns(true);
            _ordersRepositoryMock.Setup(r => r.GetOrder(accountId, orderId)).Returns(order);

            // Act
            var response = _sut.GetOrderForAccount(accountId, orderId);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(response.Result);
            var returnValue = Assert.IsType<OrderReadDto>(okObjectResult.Value);
            
            Assert.Equal(order.AccountId, returnValue.AccountId);
            Assert.Equal(order.Cost, returnValue.Cost);
            Assert.Equal(order.Id, returnValue.Id);
        }

    }
}
