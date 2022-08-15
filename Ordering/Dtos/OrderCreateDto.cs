using System.ComponentModel.DataAnnotations;

namespace Ordering.Dtos
{
    public class OrderCreateDto
    {
        [Required]
        public decimal Cost { get; set; }
    }
}