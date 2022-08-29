using AutoMapper;
using Ordering.Data;
using Ordering.Dtos;
using Ordering.Models;
using System.Text.Json;

namespace Ordering.EventProcessing
{
    public class AccountPublishedEventHandler : IEventHandler
    {
        public EventType EventType => EventType.AccountPublished;

        private readonly IMapper _mapper;
        private readonly IOrdersRepository _ordersRepository;

        public AccountPublishedEventHandler(IMapper mapper, IOrdersRepository ordersRepository)
        {
            _mapper = mapper;
            _ordersRepository = ordersRepository;
        }

        public void Handle(string message)
        {
            var accountPublishedDto = JsonSerializer.Deserialize<AccountPublishedDto>(message);
            var account = _mapper.Map<Account>(accountPublishedDto);

            AddAccount(account);
        }

        private void AddAccount(Account account)
        {
            try
            {
                if (!_ordersRepository.ExternalAccountExists(account.ExternalId))
                {
                    _ordersRepository.CreateAccount(account);
                    _ordersRepository.SaveChanges();

                    Console.WriteLine("--> Account added");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not add account to db {ex.Message}");
            }
        }
    }
}