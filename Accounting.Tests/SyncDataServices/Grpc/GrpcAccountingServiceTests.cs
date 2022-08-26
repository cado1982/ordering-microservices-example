using Accounting.Data;
using Accounting.Models;
using Accounting.Profiles;
using Accounting.SyncDataServices.Grpc;
using Grpc.Core;
using Grpc.Core.Testing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Tests.SyncDataServices.Grpc
{
    public class GrpcAccountingServiceTests
    {
        private GrpcAccountingService? _sut;

        private Mapper _mapper;
        private Mock<IAccountingRepository> _accountingRepository = new Mock<IAccountingRepository>();

        public GrpcAccountingServiceTests()
        {
            var autoMapperProfile = new AccountsProfile();
            var config = new MapperConfiguration(cfg => cfg.AddProfile(autoMapperProfile));
            _mapper = new Mapper(config);
        }

        [MemberNotNull(nameof(_sut))]
        private void CreateSUT()
        {
            _sut = new GrpcAccountingService(_accountingRepository.Object, _mapper);
        }

        [Fact]
        public async Task GetAllAccounts_ReturnsCorrectAccounts()
        {
            // Arrange
            CreateSUT();
            var account = new Account { Id = 10 };
            var request = new GetAllAccountsRequest();
            var context = TestServerCallContext.Create("GetAllAccounts",
                "localhost",
                deadline: DateTime.Now.AddMinutes(30),
                requestHeaders: new Metadata(),
                cancellationToken: CancellationToken.None,
                peer: "10.0.0.25:5001",
                authContext: null,
                contextPropagationToken: null,
                writeHeadersFunc: (metadata) => Task.CompletedTask,
                writeOptionsGetter: () => new WriteOptions(),
                writeOptionsSetter: (writeOptions) => { });
            _accountingRepository.Setup(r => r.GetAccounts()).Returns(new List<Account> { account });


            // Act
            var response = await _sut.GetAllAccounts(request, context);

            // Assert
            Assert.NotNull(response);
            var grpcAccountModel = Assert.Single(response.Accounts);
            Assert.Equal(10, grpcAccountModel.Id);



        }
    }
}
