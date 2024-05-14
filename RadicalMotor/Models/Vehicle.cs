using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadicalMotor.Models
{
	public class Vehicle
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public string ChassisNumber { get; set; }

		[Required]
		[MaxLength(255)]
		public string VehicleName { get; set; }

		public DateTime EntryDate { get; set; }

		[MaxLength(100)]
		public string Version { get; set; }

		[ForeignKey("PriceList")]
		public string PriceListID { get; set; }

		[ForeignKey("VehicleType")]
		public string VehicleTypeId { get; set; }

		public VehicleType VehicleType { get; set; }
		public PriceList PriceList { get; set; }

		public ICollection<VehicleImage>? VehicleImages { get; set; }

	}
}
