using ints.Data;
using ints.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ints.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly IDbContextFactory<AppDbContext> _factory;

        public DatabaseService(IDbContextFactory<AppDbContext> factory)
        {
            _factory = factory;
        }

        public async Task EnsureCreatedAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();
            await db.Database.EnsureCreatedAsync();
        }

        // ---------------- Cars ----------------
        public async Task<List<Car>> GetCarsAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Cars
                .AsNoTracking()
                .OrderBy(c => c.Id)
                .ToListAsync();
        }

        public async Task<Car?> GetCarByIdAsync(int id)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Cars.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddCarAsync(Car car)
        {
            await using var db = await _factory.CreateDbContextAsync();
            db.Cars.Add(car);
            await db.SaveChangesAsync();
        }

        public async Task UpdateCarAsync(Car car)
        {
            await using var db = await _factory.CreateDbContextAsync();
            db.Cars.Update(car);
            await db.SaveChangesAsync();
        }

        public async Task DeleteCarAsync(int id)
        {
            await using var db = await _factory.CreateDbContextAsync();

            // если есть водители/перевозки — запретим удаление, иначе FK упадёт
            var hasDrivers = await db.Drivers.AnyAsync(d => d.CarId == id);
            if (hasDrivers)
                throw new InvalidOperationException("Нельзя удалить автомобиль: к нему привязаны водители.");

            var hasShipments = await db.Shipments.AnyAsync(s => s.CarId == id);
            if (hasShipments)
                throw new InvalidOperationException("Нельзя удалить автомобиль: по нему есть перевозки.");

            var car = await db.Cars.FindAsync(id);
            if (car == null) return;

            db.Cars.Remove(car);
            await db.SaveChangesAsync();
        }

        // ---------------- Drivers ----------------
        public async Task<List<Driver>> GetDriversAsync(bool includeCar = true)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var q = db.Drivers.AsNoTracking().AsQueryable();
            if (includeCar) q = q.Include(d => d.Car);

            return await q.OrderBy(d => d.Id).ToListAsync();
        }

        public async Task<List<Driver>> GetDriversByCarIdAsync(int carId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Drivers
                .AsNoTracking()
                .Where(d => d.CarId == carId)
                .OrderBy(d => d.Id)
                .ToListAsync();
        }

        public async Task AddDriverAsync(Driver driver)
        {
            await using var db = await _factory.CreateDbContextAsync();

            // условие задачи: водитель закреплён только за одним авто — это и есть CarId
            // Проверим, что машина существует
            var carExists = await db.Cars.AnyAsync(c => c.Id == driver.CarId);
            if (!carExists)
                throw new InvalidOperationException("Указанный автомобиль не существует.");

            db.Drivers.Add(driver);
            await db.SaveChangesAsync();
        }

        public async Task UpdateDriverAsync(Driver driver)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var carExists = await db.Cars.AnyAsync(c => c.Id == driver.CarId);
            if (!carExists)
                throw new InvalidOperationException("Указанный автомобиль не существует.");

            db.Drivers.Update(driver);
            await db.SaveChangesAsync();
        }

        public async Task DeleteDriverAsync(int id)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var hasShipments = await db.Shipments.AnyAsync(s => s.DriverId == id);
            if (hasShipments)
                throw new InvalidOperationException("Нельзя удалить водителя: по нему есть перевозки.");

            var driver = await db.Drivers.FindAsync(id);
            if (driver == null) return;

            db.Drivers.Remove(driver);
            await db.SaveChangesAsync();
        }

        // ---------------- Fuel types ----------------
        public async Task<List<FuelType>> GetFuelTypesAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.FuelTypes
                .AsNoTracking()
                .OrderBy(f => f.Id)
                .ToListAsync();
        }

        public async Task AddFuelTypeAsync(FuelType fuelType)
        {
            await using var db = await _factory.CreateDbContextAsync();
            db.FuelTypes.Add(fuelType);
            await db.SaveChangesAsync();
        }

        public async Task UpdateFuelTypeAsync(FuelType fuelType)
        {
            await using var db = await _factory.CreateDbContextAsync();
            db.FuelTypes.Update(fuelType);
            await db.SaveChangesAsync();
        }

        public async Task DeleteFuelTypeAsync(int id)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var hasShipments = await db.Shipments.AnyAsync(s => s.FuelTypeId == id);
            if (hasShipments)
                throw new InvalidOperationException("Нельзя удалить марку топлива: по ней есть перевозки.");

            var fuel = await db.FuelTypes.FindAsync(id);
            if (fuel == null) return;

            db.FuelTypes.Remove(fuel);
            await db.SaveChangesAsync();
        }

        // ---------------- Shipments ----------------
        public async Task<List<Shipment>> GetShipmentsAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.Shipments
                .AsNoTracking()
                .Include(s => s.Car)
                .Include(s => s.Driver)
                .Include(s => s.FuelType)
                .OrderByDescending(s => s.Id)
                .ToListAsync();
        }

        public async Task AddShipmentAsync(Shipment shipment)
        {
            await using var db = await _factory.CreateDbContextAsync();

            // Проверка условия: водитель должен быть закреплён за выбранным авто
            var driverCarId = await db.Drivers
                .Where(d => d.Id == shipment.DriverId)
                .Select(d => (int?)d.CarId)
                .SingleOrDefaultAsync();

            if (driverCarId == null)
                throw new InvalidOperationException("Водитель не найден.");

            if (driverCarId.Value != shipment.CarId)
                throw new InvalidOperationException("Водитель закреплён за другим автомобилем.");

            // Проверим, что топливо существует
            var fuelExists = await db.FuelTypes.AnyAsync(f => f.Id == shipment.FuelTypeId);
            if (!fuelExists)
                throw new InvalidOperationException("Марка топлива не найдена.");

            db.Shipments.Add(shipment);
            await db.SaveChangesAsync();
        }

        public async Task DeleteShipmentAsync(int id)
        {
            await using var db = await _factory.CreateDbContextAsync();
            var shipment = await db.Shipments.FindAsync(id);
            if (shipment == null) return;

            db.Shipments.Remove(shipment);
            await db.SaveChangesAsync();
        }
    }
}
