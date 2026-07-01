using AutoMapper;
using ints.Models;
using intsAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace intsAPI.Services
{
    public class ShipmentService : IShipmentService
    {
        private readonly IntsContext _context;
        private readonly IMapper _mapper;
        private readonly ICarService _carService;
        private readonly IDriverService _driverService;
        private readonly IFuelTypeService _fuelTypeService;

        public ShipmentService(
            IntsContext context,
            IMapper mapper,
            ICarService carService,
            IDriverService driverService,
            IFuelTypeService fuelTypeService)
        {
            _context = context;
            _mapper = mapper;
            _carService = carService;
            _driverService = driverService;
            _fuelTypeService = fuelTypeService;
        }

        public async Task<List<ShipmentDto>> GetAllAsync()
        {
            var shipments = await _context.Shipments
                .AsNoTracking()
                .Include(s => s.Car)
                .Include(s => s.Driver)
                .Include(s => s.FuelType)
                .OrderByDescending(s => s.Id)
                .ToListAsync();

            return _mapper.Map<List<ShipmentDto>>(shipments);
        }

        public async Task<ShipmentDto?> GetByIdAsync(int id)
        {
            var shipment = await _context.Shipments
                .AsNoTracking()
                .Include(s => s.Car)
                .Include(s => s.Driver)
                .Include(s => s.FuelType)
                .FirstOrDefaultAsync(s => s.Id == id);

            return shipment == null ? null : _mapper.Map<ShipmentDto>(shipment);
        }

        public async Task<ShipmentDto> CreateAsync(CreateShipmentRequest request)
        {
            await ValidateReferencesAsync(request.CarId, request.DriverId, request.FuelTypeId);

            var shipment = _mapper.Map<Shipment>(request);
            _context.Shipments.Add(shipment);
            await _context.SaveChangesAsync();
            return _mapper.Map<ShipmentDto>(shipment);
        }

        public async Task<bool> UpdateAsync(int id, UpdateShipmentRequest request)
        {
            var shipment = await _context.Shipments.FindAsync(id);
            if (shipment == null) return false;

            await ValidateReferencesAsync(request.CarId, request.DriverId, request.FuelTypeId);

            _mapper.Map(request, shipment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var shipment = await _context.Shipments.FindAsync(id);
            if (shipment == null) return false;

            _context.Shipments.Remove(shipment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Shipments.AnyAsync(s => s.Id == id);
        }

        private async Task ValidateReferencesAsync(int carId, int driverId, int fuelTypeId)
        {
            if (!await _carService.ExistsAsync(carId))
                throw new InvalidOperationException($"Car with ID {carId} does not exist");

            if (!await _driverService.ExistsAsync(driverId))
                throw new InvalidOperationException($"Driver with ID {driverId} does not exist");

            if (!await _fuelTypeService.ExistsAsync(fuelTypeId))
                throw new InvalidOperationException($"FuelType with ID {fuelTypeId} does not exist");
        }
    }
}

