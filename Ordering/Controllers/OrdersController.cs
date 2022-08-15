using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Ordering.Data;
using Ordering.Dtos;
using Ordering.Models;

namespace Ordering.Controllers
{
    [Route("api/accounts/{accountId}/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersRepository _repository;
        private readonly IMapper _mapper;

        public OrdersController(IOrdersRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet("{orderId}", Name = nameof(GetOrderForAccount))]
        public ActionResult<OrderReadDto> GetOrderForAccount(int accountId, int orderId)
        {
            if (!_repository.AccountExists(accountId))
            {
                return NotFound();
            }

            var order = _repository.GetOrder(orderId, accountId);

            if (order == null)
            {
                return NotFound();
            }

            var orderReadDto = _mapper.Map<OrderReadDto>(order);

            return Ok(orderReadDto);
        }


        [HttpPost]
        public ActionResult<OrderReadDto> CreateOrder(int accountId, OrderCreateDto orderCreateDto)
        {
            if (!_repository.AccountExists(accountId))
            {
                return NotFound();
            }

            var order = _mapper.Map<Order>(orderCreateDto);

            _repository.CreateOrder(accountId, order);
            _repository.SaveChanges();

            var orderReadDto = _mapper.Map<OrderReadDto>(order);

            return CreatedAtRoute(nameof(GetOrderForAccount), new { accountId = accountId, orderId = orderReadDto.Id}, orderReadDto);


        }
    }
}