using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accounting;
using AutoMapper;
using Grpc.Net.Client;
using Ordering.Models;

namespace Ordering.SyncDataService.Grpc
{
    public class AccountsDataClient : IAccountsDataClient
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public AccountsDataClient(IConfiguration config, IMapper mapper)
        {
            _config = config;
            _mapper = mapper;
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            var channel = GrpcChannel.ForAddress(_config["GrpcAccountingService"]);
            var client = new GrpcAccountsService.GrpcAccountsServiceClient(channel);
            var request = new GetAllAccountsRequest();

            try
            {
                var reply = client.GetAllAccounts(request);
                return _mapper.Map<IEnumerable<Account>>(reply.Accounts);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not call GRPC server {ex.Message}");
                return Enumerable.Empty<Account>();
            }
        }
    }
}