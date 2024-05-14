﻿using System.ComponentModel.DataAnnotations;

namespace RadicalMotor.Models
{
    public class Service
    {
        [Key]
        public string ServiceId { get; set; }

        [Required]
        [MaxLength(255)]
        public string ServiceName { get; set; }

        public string ServiceDescription { get; set; }

        public decimal ServicePrice { get; set; }
        public List<AppointmentDetail> AppointmentDetails { get; set; }
    }
}
