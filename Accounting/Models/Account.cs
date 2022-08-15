using System.ComponentModel.DataAnnotations;

namespace Accounting.Models
{
    public class Account
    {
        [Key]
        [Required]
        public int Id { get; set; }
    }
}