using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadicalMotor.Models
{
    public class CustomerImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ImageId { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        [ForeignKey("Customer")]
        public string CustomerId { get; set; }

        public Customer Customer { get; set; }
    }
}
