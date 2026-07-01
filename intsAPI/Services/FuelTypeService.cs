using AutoMapper;
using ints.Models;
using Microsoft.EntityFrameworkCore;

namespace intsAPI.Services
{
    public class FuelTypeService : IFuelTypeService
    {
        private readonly IntsContext _context;
        private readonly IMapper _mapper;

        public FuelTypeService(IntsContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<FuelTypeDto>> GetAllAsync()
        {
            var fuelTypes = await _context.FuelTypes
                .AsNoTracking()
                .Include(f => f.Shipments)
                .OrderByDescending(f => f.Id)
                .ToListAsync();

            return _mapper.Map<List<FuelTypeDto>>(fuelTypes);
        }

        public async Task<FuelTypeDto?> GetByIdAsync(int id)
        {
            var fuelType = await _context.FuelTypes
                .AsNoTracking()
                .Include(f => f.Shipments)
                .FirstOrDefaultAsync(f => f.Id == id);

            return fuelType == null ? null : _mapper.Map<FuelTypeDto>(fuelType);
        }

        public async Task<FuelTypeDto> CreateAsync(CreateFuelTypeRequest request)
        {
            var fuelType = _mapper.Map<FuelType>(request);
            _context.FuelTypes.Add(fuelType);
            await _context.SaveChangesAsync();
            return _mapper.Map<FuelTypeDto>(fuelType);
        }

        public async Task<bool> UpdateAsync(int id, UpdateFuelTypeRequest request)
        {
            var fuelType = await _context.FuelTypes.FindAsync(id);
            if (fuelType == null) return false;

            _mapper.Map(request, fuelType);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var fuelType = await _context.FuelTypes.FindAsync(id);
            if (fuelType == null) return false;

            _context.FuelTypes.Remove(fuelType);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.FuelTypes.AnyAsync(f => f.Id == id);
        }
    }
}

