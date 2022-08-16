using Accounting;
using AutoMapper;
using Ordering.Dtos;
using Ordering.Models;

namespace Ordering.Profiles
{
    public class OrdersProfile : Profile
    {
        public OrdersProfile()
        {
            CreateMap<Account, AccountReadDto>();
            CreateMap<Order, OrderReadDto>();
            CreateMap<OrderCreateDto, Order>();

            CreateMap<GrpcAccountModel, Account>()
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Orders, opt => opt.Ignore());
        }
    }
}