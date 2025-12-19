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
    public interface IDbService
    {
        Task<List<Car>> GetCarsAsync();
        Task<List<Driver>> GetDriversAsync();
        Task<List<FuelType>> GetFuelTypesAsync();
        Task<List<Shipment>> GetShipmentsAsync();

        Task EnsureCreatedAsync();

        Task AddCarAsync(Car car);
        Task AddDriverAsync(Driver driver);
        Task AddFuelTypeAsync(FuelType fuel);
        Task AddShipmentAsync(Shipment shipment);

        Task DeleteCarAsync(int id);
        Task DeleteDriverAsync(int id);
        Task DeleteFuelTypeAsync(int id);
        Task DeleteShipmentAsync(int id);
    }

    public class DbService : IDbService
    {
        private readonly IDbContextFactory<AppDbContext> _factory;

        public DbService(IDbContextFactory<AppDbContext> factory) => _factory = factory;

        public async Task EnsureCreatedAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();
            await db.Database.EnsureCreatedAsync();
        }

        public async Task<List<Car>> GetCarsAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Cars.AsNoTracking().OrderBy(c => c.RegNumber).ToListAsync();
        }

        public async Task<List<Driver>> GetDriversAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Drivers.AsNoTracking().Include(d => d.Car).OrderBy(d => d.FullName).ToListAsync();
        }

        public async Task<List<FuelType>> GetFuelTypesAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.FuelTypes.AsNoTracking().OrderBy(f => f.Name).ToListAsync();
        }

        public async Task<List<Shipment>> GetShipmentsAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Shipments.AsNoTracking()
                .Include(s => s.Car)
                .Include(s => s.Driver)
                .Include(s => s.FuelType)
                .OrderByDescending(s => s.Date)
                .ToListAsync();
        }

        public async Task AddCarAsync(Car car)
        {
            await using var db = await _factory.CreateDbContextAsync();
            db.Cars.Add(car);
            await db.SaveChangesAsync();
        }

        public async Task AddDriverAsync(Driver driver)
        {
            await using var db = await _factory.CreateDbContextAsync();
            db.Drivers.Add(driver);
            await db.SaveChangesAsync();
        }

        public async Task AddFuelTypeAsync(FuelType fuel)
        {
            await using var db = await _factory.CreateDbContextAsync();
            db.FuelTypes.Add(fuel);
            await db.SaveChangesAsync();
        }

        public async Task AddShipmentAsync(Shipment shipment)
        {
            await using var db = await _factory.CreateDbContextAsync();

            // Валидация: водитель должен быть закреплен за выбранным авто
            var driverCarId = await db.Drivers.Where(d => d.Id == shipment.DriverId)
                .Select(d => d.CarId)
                .SingleAsync();

            if (driverCarId != shipment.CarId)
                throw new InvalidOperationException("Водитель закреплён за другим автомобилем.");

            db.Shipments.Add(shipment);
            await db.SaveChangesAsync();
        }

        public async Task DeleteCarAsync(int id)
        {
            await using var db = await _factory.CreateDbContextAsync();
            var car = await db.Cars.FindAsync(id);
            if (car != null)
            {
                db.Cars.Remove(car);
                await db.SaveChangesAsync();
            }
        }

        public async Task DeleteDriverAsync(int id)
        {
            await using var db = await _factory.CreateDbContextAsync();
            var d = await db.Drivers.FindAsync(id);
            if (d != null)
            {
                db.Drivers.Remove(d);
                await db.SaveChangesAsync();
            }
        }

        public async Task DeleteFuelTypeAsync(int id)
        {
            await using var db = await _factory.CreateDbContextAsync();
            var f = await db.FuelTypes.FindAsync(id);
            if (f != null)
            {
                db.FuelTypes.Remove(f);
                await db.SaveChangesAsync();
            }
        }

        public async Task DeleteShipmentAsync(int id)
        {
            await using var db = await _factory.CreateDbContextAsync();
            var s = await db.Shipments.FindAsync(id);
            if (s != null)
            {
                db.Shipments.Remove(s);
                await db.SaveChangesAsync();
            }
        }
    }
}