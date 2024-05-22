using System.ComponentModel.DataAnnotations;

namespace RadicalMotor.Models
{
    public class PriceList
    {
        [Required, Key]
        public string PriceListId { get; set; }
        public DateTime DateCreated { get; set; }
        public decimal Price { get; set; }

        public string Status { get; set; }
        public ICollection<Vehicle>? Vehicles { get; set; }
        public List<PromotionDetail>? PromotionDetails { get; set; }
    }
}
