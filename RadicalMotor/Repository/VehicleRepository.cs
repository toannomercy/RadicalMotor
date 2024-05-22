using Microsoft.EntityFrameworkCore;
using RadicalMotor.Models;

namespace RadicalMotor.Repository
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly ApplicationDbContext _context;

        public VehicleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Vehicle>> GetAllAsync()
        {
            return await _context.Vehicles
            .Include(p => p.VehicleType)
            .ToListAsync();
        }
        public async Task<Vehicle> GetByIdAsync(string id)
        {
            return await _context.Vehicles.Include(p => p.VehicleType).FirstOrDefaultAsync(p => p.ChassisNumber == id);
        }

        public async Task AddAsync(Vehicle vehicle)
        {
            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();
        }

        public Vehicle AddVehicle(Vehicle vehicle)
        {
            _context.Vehicles.Add(vehicle);
            _context.SaveChanges();
            return vehicle;
        }

        public Vehicle UpdateVehicle(Vehicle vehicle, List<VehicleImage> images)
        {
            _context.Vehicles.Update(vehicle);
            var existingImages = _context.VehicleImages.Where(img => img.ChassisNumber == vehicle.ChassisNumber).ToList();
            _context.VehicleImages.RemoveRange(existingImages);
            _context.VehicleImages.AddRange(images);
            _context.SaveChanges();
            return vehicle;
        }

        public void DeleteVehicle(string chassisNumber)
        {
            var vehicle = _context.Vehicles.Find(chassisNumber);
            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
                _context.SaveChanges();
            }
        }
        public async Task<IEnumerable<Vehicle>> GetAllWithPriceListAsync()
        {
            return await _context.Vehicles
                                 .Include(v => v.PriceList)
                                 .Include(v => v.VehicleType)
                                 .Include(v => v.VehicleImages)
                                 .ToListAsync();
        }
    }
}
