using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadicalMotor.Models
{
    public class Vehicle
    {
        [Key]
        public string ChassisNumber { get; set; }

        [Required]
        [MaxLength(255)]
        public string VehicleName { get; set; }

        [DataType(DataType.Date)]
        public DateTime EntryDate { get; set; }

        [MaxLength(100)]
        public string Version { get; set; }

        [ForeignKey("PriceList")]
        public string PriceListID { get; set; }

        [ForeignKey("VehicleType")]
        public string VehicleTypeId { get; set; }
        [DisplayName("VehicleImage")]
        public VehicleType? VehicleType { get; set; }
        public PriceList? PriceList { get; set; }

        [NotMapped]
        public List<IFormFile>? Images { get; set; }

        public ICollection<VehicleImage>? VehicleImages { get; set; }
    }
}
