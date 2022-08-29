using Accounting.Dtos;
using Accounting.Models;
using AutoMapper;

namespace Accounting.Profiles
{
    public class AccountsProfile : Profile
    {
        public AccountsProfile()
        {
            CreateMap<Account, AccountReadDto>();
            CreateMap<AccountCreateDto, Account>();
            CreateMap<AccountReadDto, AccountPublishedDto>();
            CreateMap<Account, GrpcAccountModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
        }
    }
}