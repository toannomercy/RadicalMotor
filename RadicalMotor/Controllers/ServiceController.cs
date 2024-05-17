using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RadicalMotor.DTO;
using RadicalMotor.Models;
using RadicalMotor.Repository;
using System.Linq;
using System.Threading.Tasks;

namespace RadicalMotor.Controllers
{
	public class ServiceController : Controller
	{
		private readonly IAppointmentRepository _appointmentRepository;
		private readonly ICustomerRepository _customerRepository;
		private readonly UserManager<ApplicationUser> _userManager;

		public ServiceController(IAppointmentRepository appointmentRepository, ICustomerRepository customerRepository, UserManager<ApplicationUser> userManager)
		{
			_appointmentRepository = appointmentRepository;
			_customerRepository = customerRepository;
			_userManager = userManager;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var services = await _appointmentRepository.GetAllServicesAsync();
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
			var customer = await _customerRepository.GetCustomerByUserIdAsync(user.Id);

			if (customer == null)
			{
				return Json(new { success = false, message = "Customer not found." });
			}

			if (customer.PhoneNumber != model.PhoneNumber)
			{
				return Json(new { success = false, message = "Phone number does not match with the logged-in user." });
			}

			var service = await _appointmentRepository.GetServiceByIdAsync(model.SelectedServiceId);
			if (service == null)
			{
				return Json(new { success = false, message = "Service not found." });
			}

			var adminEmployee = await _appointmentRepository.GetAdminEmployeeAsync(); 
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

			await _appointmentRepository.AddAppointmentAsync(appointment);

			var appointmentDetail = new AppointmentDetail
			{
				AppointmentId = appointment.AppointmentId,
				ServiceId = model.SelectedServiceId,
				ServiceAmount = service.ServicePrice
			};

			await _appointmentRepository.AddAppointmentDetailAsync(appointmentDetail);

			return Json(new { success = true, message = "Your appointment has been booked successfully." });
		}
	}
}
