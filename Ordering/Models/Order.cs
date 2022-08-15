using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.Models
{
    public class Order
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public decimal Cost { get; set; }

        [Required]
        public int AccountId { get; set; }

        public Account Account { get; set; }
    }
}