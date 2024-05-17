using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadicalMotor.DTO;
using RadicalMotor.Models;

namespace RadicalMotor.Controllers
{
    public class ServiceController : Controller
    {
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
		public ServiceController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var services = await _context.Services.ToListAsync();
			return View(new AppointmentDTO { Services = services });
		}
		[HttpPost]
		public async Task<IActionResult> BookAppointment([FromBody] AppointmentDetailDTO model)
		{
			if (!ModelState.IsValid)
			{
				return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
			}

			var user = await _userManager.GetUserAsync(User);
			var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == user.Id);

			if (customer == null)
			{
				return Json(new { success = false, message = "Customer not found." });
			}

			if (customer.PhoneNumber != model.PhoneNumber)
			{
				return Json(new { success = false, message = "Phone number does not match with the logged-in user." });
			}

			var service = await _context.Services.FirstOrDefaultAsync(s => s.ServiceId == model.SelectedServiceId);
			if (service == null)
			{
				return Json(new { success = false, message = "Service not found." });
			}

			var adminEmployee = await _context.Employees.FirstOrDefaultAsync(e => e.Position == "Admin");
			if (adminEmployee == null)
			{
				return Json(new { success = false, message = "No Admin employee found." });
			}
			var appointment = new Appointment
			{
				CustomerID = customer.CustomerId,
				EmployeeID = adminEmployee.EmployeeId,
				DateCreated = model.ServiceDate,
				TotalAmount = service.ServicePrice,
				Content = model.Notes
			};

			_context.Appointments.Add(appointment);
			await _context.SaveChangesAsync();

			var appointmentDetail = new AppointmentDetail
			{
				AppointmentId = appointment.AppointmentId,
				ServiceId = model.SelectedServiceId,
				ServiceAmount = service.ServicePrice,
			};

			_context.AppointmentDetails.Add(appointmentDetail);
			await _context.SaveChangesAsync();

			return Json(new { success = true, message = "Your appointment has been booked successfully." });
		}
	}
}
