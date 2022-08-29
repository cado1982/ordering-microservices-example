using Accounting.Data;
using AutoMapper;
using Grpc.Core;
using static Accounting.GrpcAccountsService;

namespace Accounting.SyncDataServices.Grpc
{
    public class GrpcAccountingService : GrpcAccountsServiceBase
    {
        private readonly IAccountingRepository _repository;
        private readonly IMapper _mapper;

        public GrpcAccountingService(IAccountingRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public override Task<GetAllAccountsResponse> GetAllAccounts(GetAllAccountsRequest request, ServerCallContext context)
        {
            var response = new GetAllAccountsResponse();
            var accounts = _repository.GetAccounts();

            foreach (var account in accounts)
            {
                var grpcAccountModel = _mapper.Map<GrpcAccountModel>(account);
                response.Accounts.Add(grpcAccountModel);
            }

            return Task.FromResult(response);
        }

    }
}