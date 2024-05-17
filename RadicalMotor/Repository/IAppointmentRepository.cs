using RadicalMotor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RadicalMotor.Repository
{
	public interface IAppointmentRepository
	{
		Task<List<Appointment>> GetAllAppointmentsAsync();
		Task<Appointment> GetAppointmentByIdAsync(string id);
		Task AddAppointmentAsync(Appointment appointment);
		Task UpdateAppointmentAsync(Appointment appointment);
		Task DeleteAppointmentAsync(string id);
		Task AddAppointmentDetailAsync(AppointmentDetail appointmentDetail);
		Task<List<Service>> GetAllServicesAsync();
		Task<Service> GetServiceByIdAsync(string id);
		Task<Employee> GetAdminEmployeeAsync();
	}
}
