using Microsoft.EntityFrameworkCore;
using RadicalMotor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RadicalMotor.Repository
{
	public class AppointmentRepository : IAppointmentRepository
	{
		private readonly ApplicationDbContext _context;

		public AppointmentRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<List<Appointment>> GetAllAppointmentsAsync()
		{
			return await _context.Appointments.ToListAsync();
		}

		public async Task<Appointment> GetAppointmentByIdAsync(string id)
		{
			return await _context.Appointments.FindAsync(id);
		}

		public async Task AddAppointmentAsync(Appointment appointment)
		{
			_context.Appointments.Add(appointment);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAppointmentAsync(Appointment appointment)
		{
			_context.Appointments.Update(appointment);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAppointmentAsync(string id)
		{
			var appointment = await _context.Appointments.FindAsync(id);
			if (appointment != null)
			{
				_context.Appointments.Remove(appointment);
				await _context.SaveChangesAsync();
			}
		}

		public async Task AddAppointmentDetailAsync(AppointmentDetail appointmentDetail)
		{
			_context.AppointmentDetails.Add(appointmentDetail);
			await _context.SaveChangesAsync();
		}

		public async Task<List<Service>> GetAllServicesAsync()
		{
			return await _context.Services.ToListAsync();
		}

		public async Task<Service> GetServiceByIdAsync(string id)
		{
			return await _context.Services.FindAsync(id);
		}
		public async Task<Employee> GetAdminEmployeeAsync()
		{
			return await _context.Employees.FirstOrDefaultAsync(e => e.Position == "Admin");
		}
	}
}
