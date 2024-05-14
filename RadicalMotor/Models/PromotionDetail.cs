using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadicalMotor.Models
{
    public class PromotionDetail
    {
        [Key]
        [ForeignKey("PriceList")]
        public string PriceListId { get; set; }
        [Key]
        [ForeignKey("Promotion")]
        public string PromotionId { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal DiscountAmount { get; set; }

        public PriceList PriceList { get; set; }
        public Promotion Promotion { get; set; }
    }
}
