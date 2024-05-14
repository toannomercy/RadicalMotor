using RadicalMotor.Models;

public interface IVehicleImageRepository
{
    Task<IEnumerable<VehicleImage>> GetAll();
    Task<VehicleImage> GetById(int id);
    Task<IEnumerable<VehicleImage>> GetByChassisNumber(string chassisNumber);
    Task Add(VehicleImage vehicleImage);
    Task Update(VehicleImage vehicleImage);
    Task Delete(int id);
}
