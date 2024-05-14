using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadicalMotor.Models
{
    public class InstallmentContract
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string InstallmentContractId { get; set; }
        public DateTime? DateCreated { get; set; }
        public string? Content { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal MonthlyInstallment { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpectedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        [ForeignKey("EmployeeCreate")]
        public string EmployeeId { get; set; }
        [ForeignKey("CustomerCreate")]
        public string CustomerId { get; set; }

        [ForeignKey("VehicleBought")]
        public string ChassisNumber { get; set; }
        [ForeignKey("InstallmentPlan")]
        public string InstallmentPlanId { get; set; }

        public Employee EmployeeCreate { get; set; }
        public Customer CustomerCreate { get; set; }
        public Vehicle VehicleBought { get; set; }
        public InstallmentPlan InstallmentPlan { get; set; }
    }
}
