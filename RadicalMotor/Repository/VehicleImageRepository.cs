using Microsoft.EntityFrameworkCore;
using RadicalMotor.Models;

namespace RadicalMotor.Repository
{
    public class VehicleImageRepository : IVehicleImageRepository
    {
        private readonly ApplicationDbContext _context;

        public VehicleImageRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<VehicleImage>> GetByChassisNumber(string chassisNumber)
        {
            return await _context.Set<VehicleImage>().Where(v => v.ChassisNumber == chassisNumber).ToListAsync();
        }


        public async Task<IEnumerable<VehicleImage>> GetAll()
        {
            return await _context.Set<VehicleImage>().ToListAsync();
        }

        public async Task<VehicleImage> GetById(int id)
        {
            var vehicleImage = await _context.Set<VehicleImage>().FindAsync(id);
            if (vehicleImage == null)
            {
                throw new NullReferenceException($"No vehicle image found with the id {id}");
            }
            return vehicleImage;
        }

        public async Task Add(VehicleImage vehicleImage)
        {
            _context.Set<VehicleImage>().Add(vehicleImage);
            await _context.SaveChangesAsync();
        }

        public async Task Update(VehicleImage vehicleImage)
        {
            _context.Entry(vehicleImage).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var vehicleImage = await _context.Set<VehicleImage>().FindAsync(id);
            if (vehicleImage != null)
            {
                _context.Set<VehicleImage>().Remove(vehicleImage);
                await _context.SaveChangesAsync();
            }
        }
    }


}