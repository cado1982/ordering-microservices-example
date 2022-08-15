using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.Dtos
{
    public class OrderReadDto
    {
        public int Id { get; set; }
        public decimal Cost { get; set; }
        public int AccountId { get; set; }
    }
}