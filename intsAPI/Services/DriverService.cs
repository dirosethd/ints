using AutoMapper;
using ints.Models;
using intsAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace intsAPI.Services
{
    public class DriverService
    {
        public class DriverService : IDriverService
        {
            private readonly IntsContext _context;
            private readonly IMapper _mapper;
            private readonly ICarService _carService;

            public DriverService(IntsContext context, IMapper mapper, ICarService carService)
            {
                _context = context;
                _mapper = mapper;
                _carService = carService;
            }

            public async Task<List<DriverDto>> GetAllAsync()
            {
                var drivers = await _context.Drivers
                    .AsNoTracking()
                    .Include(d => d.Car)
                    .Include(d => d.Shipments)
                    .OrderByDescending(d => d.Id)
                    .ToListAsync();

                return _mapper.Map<List<DriverDto>>(drivers);
            }

            public async Task<DriverDto?> GetByIdAsync(int id)
            {
                var driver = await _context.Drivers
                    .AsNoTracking()
                    .Include(d => d.Car)
                    .Include(d => d.Shipments)
                    .FirstOrDefaultAsync(d => d.Id == id);

                return driver == null ? null : _mapper.Map<DriverDto>(driver);
            }

            public async Task<DriverDto> CreateAsync(CreateDriverRequest request)
            {
                if (!await _carService.ExistsAsync(request.CarId))
                    throw new InvalidOperationException($"Car with ID {request.CarId} does not exist");

                var driver = _mapper.Map<Driver>(request);
                _context.Drivers.Add(driver);
                await _context.SaveChangesAsync();
                return _mapper.Map<DriverDto>(driver);
            }

            public async Task<bool> UpdateAsync(int id, UpdateDriverRequest request)
            {
                var driver = await _context.Drivers.FindAsync(id);
                if (driver == null) return false;

                if (!await _carService.ExistsAsync(request.CarId))
                    throw new InvalidOperationException($"Car with ID {request.CarId} does not exist");

                _mapper.Map(request, driver);
                await _context.SaveChangesAsync();
                return true;
            }

            public async Task<bool> DeleteAsync(int id)
            {
                var driver = await _context.Drivers.FindAsync(id);
                if (driver == null) return false;

                _context.Drivers.Remove(driver);
                await _context.SaveChangesAsync();
                return true;
            }

            public async Task<bool> ExistsAsync(int id)
            {
                return await _context.Drivers.AnyAsync(d => d.Id == id);
            }
        }
    }
}
