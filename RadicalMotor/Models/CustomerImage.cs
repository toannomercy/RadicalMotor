using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RadicalMotor.Models
{
    public class CustomerImage
    {
        [Key]
        public string ImageId { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        [ForeignKey("Customer")]
        public string CustomerId { get; set; }

        public Customer Customer { get; set; }
    }
}
