using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadicalMotor.Models
{
    public class VehicleType
    {
        [Key]
        public string VehicleTypeId { get; set; }

        [Required]
        [MaxLength(255)]
        public string TypeName { get; set; }

        [ForeignKey("Supplier")]
        public string SupplierId { get; set; }

        public Supplier Supplier { get; set; }
    }
}