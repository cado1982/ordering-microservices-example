using Accounting.Data;
using Accounting.Dtos;
using Accounting.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountingRepository _accountingRepository;
        private readonly IMapper _mapper;

        public AccountsController(IAccountingRepository accountingRepository, IMapper mapper)
        {
            _accountingRepository = accountingRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<AccountReadDto>> GetAccounts()
        {
            var accountModels = _accountingRepository.GetAccounts();

            var accountDtos = _mapper.Map<IEnumerable<AccountReadDto>>(accountModels);

            return Ok(accountDtos);
        }

        [HttpGet("{id}", Name = nameof(GetAccountById))]
        public ActionResult<AccountReadDto> GetAccountById(int id)
        {
            var accountModel = _accountingRepository.GetAccountById(id);

            if (accountModel == null)
            {
                return NotFound();
            }

            var accountReadDto = _mapper.Map<AccountReadDto>(accountModel);

            return Ok(accountReadDto);
        }

        [HttpPost]
        public ActionResult<AccountReadDto> CreateAccount(AccountCreateDto accountCreateDto)
        {
            var accountModel = _mapper.Map<Account>(accountCreateDto);

            _accountingRepository.CreateAccount(accountModel);
            _accountingRepository.SaveChanges();

            var accountReadDto = _mapper.Map<AccountReadDto>(accountModel);

            return CreatedAtRoute(nameof(GetAccountById), new { Id = accountReadDto.Id }, accountReadDto);
        }
    }
}