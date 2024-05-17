using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadicalMotor.Models
{
    public class VehicleImage
    {

        [Key]
        public string ImageId { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        [ForeignKey("Vehicle")]
        public string ChassisNumber { get; set; }

        public Vehicle Vehicle { get; set; }
    }

}
