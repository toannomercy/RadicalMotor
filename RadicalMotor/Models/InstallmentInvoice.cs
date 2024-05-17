using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadicalMotor.Models
{
    public class InstallmentInvoice
    {
        [Required, Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string InstallmentInvoiceId { get; set; }
        public DateTime TransactionDate { get; set; }

        public decimal AmountPaid { get; set; }

        public string PaymentMethod { get; set; }

        public DateTime NextPaymentDate { get; set; }

        public string Content { get; set; }

        [ForeignKey("EmployeeCreate")]
        public string EmployeeId { get; set; }

        [ForeignKey("InstallmentContract")]
        public string InstallmentContractId { get; set; }
        public InstallmentContract InstallmentContract { get; set; }
        public Employee EmployeeCreate { get; set; }
    }
}
