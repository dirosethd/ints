using ints.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ints.Services
{
    public interface IDatabaseService
    {
        Task EnsureCreatedAsync();

        // Cars
        Task<List<Car>> GetCarsAsync();
        Task<Car?> GetCarByIdAsync(int id);
        Task AddCarAsync(Car car);
        Task UpdateCarAsync(Car car);
        Task DeleteCarAsync(int id);

        // Drivers
        Task<List<Driver>> GetDriversAsync(bool includeCar = true);
        Task<List<Driver>> GetDriversByCarIdAsync(int carId);
        Task AddDriverAsync(Driver driver);
        Task UpdateDriverAsync(Driver driver);
        Task DeleteDriverAsync(int id);

        // Fuel types
        Task<List<FuelType>> GetFuelTypesAsync();
        Task AddFuelTypeAsync(FuelType fuelType);
        Task UpdateFuelTypeAsync(FuelType fuelType);
        Task DeleteFuelTypeAsync(int id);

        // Shipments / перевозки
        Task<List<Shipment>> GetShipmentsAsync();
        Task AddShipmentAsync(Shipment shipment);
        Task DeleteShipmentAsync(int id);
    }
}
