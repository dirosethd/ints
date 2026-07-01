using AutoMapper;
using ints.Models;
using intsAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace intsAPI.Services
{
    public class CarService
    {
        public class СarService : ICarService
        {
            private readonly IntsContext _context;
            private readonly IMapper _mapper;

            public СarService(IntsContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<List<CarDto>> GetAllAsync()
            {
                var cars = await _context.Cars
                    .AsNoTracking()
                    .OrderByDescending(c => c.Id)
                    .ToListAsync();

                return _mapper.Map<List<CarDto>>(cars);
            }

            public async Task<CarDto?> GetByIdAsync(int id)
            {
                var car = await _context.Cars.FindAsync(id);
                return car == null ? null : _mapper.Map<CarDto>(car);
            }

            public async Task<CarDto> CreateAsync(CreateCarRequest request)
            {
                var car = _mapper.Map<Car>(request);
                _context.Cars.Add(car);
                await _context.SaveChangesAsync();
                return _mapper.Map<CarDto>(car);
            }

            public async Task<bool> UpdateAsync(int id, UpdateCarRequest request)
            {
                var car = await _context.Cars.FindAsync(id);
                if (car == null) return false;

                _mapper.Map(request, car);
                await _context.SaveChangesAsync();
                return true;
            }

            public async Task<bool> DeleteAsync(int id)
            {
                var car = await _context.Cars.FindAsync(id);
                if (car == null) return false;

                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
                return true;
            }
        }
    }
}
