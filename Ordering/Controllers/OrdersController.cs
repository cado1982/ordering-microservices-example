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

        [HttpGet]
        public ActionResult<IEnumerable<OrderReadDto>> GetOrdersForAccount(int accountId)
        {
            if (!_repository.AccountExists(accountId))
            {
                return NotFound();
            }

            var orders = _repository.GetOrdersForAccount(accountId);

            var orderReadDtos = _mapper.Map<IEnumerable<OrderReadDto>>(orders);

            return Ok(orderReadDtos);
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
            orderReadDto.AccountId = accountId;

            return CreatedAtRoute(nameof(GetOrderForAccount), new { accountId = accountId, orderId = orderReadDto.Id}, orderReadDto);


        }
    }
}