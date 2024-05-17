using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RadicalMotor.Models
{
	public class AppointmentDTO
	{
		public AppointmentDTO()
		{
			ServiceDate = DateTime.Now;
		}

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

		public List<Service> Services { get; set; }
	}
}
