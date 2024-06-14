using RadicalMotor.Models;

namespace RadicalMotor.Repository
{
    public interface IVehicleRepository
    {
        Task<IEnumerable<Vehicle>> GetAllAsync();
        Task<Vehicle> GetByIdAsync(string ChassisNumber);
        Vehicle AddVehicle(Vehicle vehicle);
        Task<Vehicle> UpdateVehicle(Vehicle vehicle);
        void DeleteVehicle(string chassisNumber);
        Task<IEnumerable<Vehicle>> GetAllWithPriceListAsync();
        Task AddAsync(Vehicle vehicle);
    }
}
