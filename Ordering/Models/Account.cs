using System.ComponentModel.DataAnnotations;

namespace Ordering.Models
{
    public class Account
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public int ExternalId { get; set; }

        public ICollection<Order>? Orders { get; set; }
    }
}