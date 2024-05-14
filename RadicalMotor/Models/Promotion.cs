﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadicalMotor.Models
{
    public class Promotion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string PromotionId { get; set; }

        [Required]
        [MaxLength(255)]
        public string PromotionName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public decimal Discount { get; set; }
        public List<PromotionDetail> PromotionDetails { get; set; }

    }
}
