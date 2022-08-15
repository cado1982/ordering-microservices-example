using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        }
    }
}