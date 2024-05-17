using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadicalMotor.Models
{
	public class ApplicationUser : IdentityUser
	{

	}

	public class Customer
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public string CustomerId { get; set; }

		[Required]
		[MaxLength(100)]
		public string FullName { get; set; }

		[MaxLength(20)]
		public string IdentityCard { get; set; }

		public DateTime DateOfBirth { get; set; }

		[MaxLength(10)]
		public string Gender { get; set; }

		[MaxLength(255)]
		public string Address { get; set; }

		[Phone]
		public string PhoneNumber { get; set; }

		[ForeignKey("User")]
		public string UserId { get; set; }
		public ApplicationUser User { get; set; }
		public List<Appointment> CreatedAppointments { get; set; }
	}

	public class Employee
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public string EmployeeId { get; set; }

		[Required]
		[MaxLength(100)]
		public string FullName { get; set; }

		public DateTime DateOfBirth { get; set; }

		[MaxLength(50)]
		public string Position { get; set; }

		[MaxLength(255)]
		public string Address { get; set; }

		[ForeignKey("User")]
		public string UserId { get; set; }
		public ApplicationUser User { get; set; }
		public List<Appointment> BrowsedAppointments { get; set; }
	}
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}
		public DbSet<Appointment> Appointments { get; set; }
		public DbSet<AppointmentDetail> AppointmentDetails { get; set; }
		public DbSet<Customer> Customers { get; set; }
		public DbSet<CustomerImage> CustomerImages { get; set; }
		public DbSet<Employee> Employees { get; set; }
		public DbSet<InstallmentContract> InstallmentContracts { get; set; }
		public DbSet<InstallmentNotification> InstallmentNotifications { get; set; }
		public DbSet<InstallmentPlan> InstallmentPlans { get; set; }
		public DbSet<InstallmentInvoice> InstallmentInvoices { get; set; }
		public DbSet<Promotion> Promotions { get; set; }
		public DbSet<PromotionDetail> PromotionDetails { get; set; }
		public DbSet<Service> Services { get; set; }
		public DbSet<Supplier> Suppliers { get; set; }
		public DbSet<VehicleImage> VehicleImages { get; set; }

		public DbSet<Vehicle> Vehicles { get; set; }
		public DbSet<VehicleType> VehicleTypes { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Appointment>()
				.HasOne(a => a.CustomerCreate)
				.WithMany(c => c.CreatedAppointments)
				.HasForeignKey(a => a.CustomerID)
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<Appointment>()
				.HasOne(a => a.EmployeeBrowse)
				.WithMany(e => e.BrowsedAppointments)
				.HasForeignKey(a => a.EmployeeID)
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<AppointmentDetail>()
			.HasKey(ad => new { ad.AppointmentId, ad.ServiceId });

			modelBuilder.Entity<AppointmentDetail>()
				.HasOne(ad => ad.Appointments)
				.WithMany(a => a.AppointmentDetails)
				.HasForeignKey(ad => ad.AppointmentId);

			modelBuilder.Entity<AppointmentDetail>()
				.HasOne(ad => ad.Services)
				.WithMany(a => a.AppointmentDetails)
				.HasForeignKey(ad => ad.ServiceId);

			modelBuilder.Entity<PromotionDetail>()
			.HasKey(pd => new { pd.PriceListId, pd.PromotionId });

			modelBuilder.Entity<PromotionDetail>()
				.HasOne(pd => pd.PriceList)
				.WithMany(pl => pl.PromotionDetails)
				.HasForeignKey(pd => pd.PriceListId);

			modelBuilder.Entity<PromotionDetail>()
				.HasOne(pd => pd.Promotion)
				.WithMany(p => p.PromotionDetails)
				.HasForeignKey(pd => pd.PromotionId);

			modelBuilder.Entity<InstallmentContract>()
				.HasOne(ic => ic.EmployeeCreate)
				.WithMany()
				.HasForeignKey(ic => ic.EmployeeId)
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<InstallmentContract>()
				.HasOne(ic => ic.CustomerCreate)
				.WithMany()
				.HasForeignKey(ic => ic.CustomerId)
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<InstallmentContract>()
				.HasOne(ic => ic.VehicleBought)
				.WithMany()
				.HasForeignKey(ic => ic.ChassisNumber)
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<InstallmentContract>()
				.HasOne(ic => ic.InstallmentPlan)
				.WithMany()
				.HasForeignKey(ic => ic.InstallmentPlanId)
				.OnDelete(DeleteBehavior.NoAction);
		}
	}
}
