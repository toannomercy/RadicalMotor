﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RadicalMotor.Models
{
	public class InstallmentPlan
	{
		[Key]
		public string InstallmentTypeId { get; set; }

		[Required]
		public int Tenure { get; set; }

		[Required]
		public decimal InterestRate { get; set; }
	}
}