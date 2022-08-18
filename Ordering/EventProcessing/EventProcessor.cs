using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Ordering.Data;
using Ordering.Dtos;
using Ordering.Models;

namespace Ordering.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }

        public void ProcessEvent(string message)
        {
            EventType eventType = DetermineEvent(message);

            switch (eventType)
            {
                case EventType.AccountPublished:
                AddAccount(message);
                    break;
            }
        }

        private EventType DetermineEvent(string eventMessage)
        {
            Console.WriteLine($"--> Determining event type {eventMessage}");

            var eventType = JsonSerializer.Deserialize<GenericEventDto>(eventMessage);

            switch(eventType.Event)
            {
                case "Account_Published":
                    return EventType.AccountPublished;
                default:
                    return EventType.Undetermined;
            }
        }

        private void AddAccount(string accountPublishedMessage)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IOrdersRepository>();
                var accountPublishedDto = JsonSerializer.Deserialize<AccountPublishedDto>(accountPublishedMessage);

                try
                {
                    var account = _mapper.Map<Account>(accountPublishedDto);

                    if (!repo.ExternalAccountExists(account.ExternalId))
                    {
                        repo.CreateAccount(account);
                        repo.SaveChanges();
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

    enum EventType
    {
        AccountPublished,
        Undetermined
    }
}