using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadicalMotor.Models
{
    public class AppointmentDetail
    {
        [Key]
        [ForeignKey("Appointments")]
        public string AppointmentId { get; set; }
        [Key]
        [ForeignKey("Services")]
        public string ServiceId { get; set; }

        public decimal ServiceAmount { get; set; }
        public Appointment Appointments { get; set; }
        public Service Services { get; set; }

    }
}
