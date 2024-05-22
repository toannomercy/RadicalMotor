namespace RadicalMotor.Models
{
    public class VehicleViewModel
    {
        public string ChassisNumber { get; set; }
        public ICollection<IFormFile> VehicleImages { get; set; }
    }
}
