using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadicalMotor.Models
{
    public class Appointment
    {
        [Required, Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public string AppointmentId { get; set; }
        public DateTime DateCreated { get; set; }
        public string? Content { get; set; }
        public decimal TotalAmount { get; set; }
        [ForeignKey("CustomerCreate")]
        public string CustomerID { get; set; }
        [ForeignKey("EmployeeBrowse")]
        public string EmployeeID { get; set; }
        public Customer CustomerCreate { get; set; }
        public Employee EmployeeBrowse { get; set; }
        public List<AppointmentDetail> AppointmentDetails { get; set; }

    }
}
