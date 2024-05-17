using System.ComponentModel.DataAnnotations;

namespace RadicalMotor.DTO
{
	public class AppointmentDetailDTO
	{
		[Required]
		public string Name { get; set; }

		[Required]
		[Phone]
		public string PhoneNumber { get; set; }

		[Required]
		public string SelectedServiceId { get; set; }

		[Required]
		public DateTime ServiceDate { get; set; }

		public string Notes { get; set; }
	}
}
