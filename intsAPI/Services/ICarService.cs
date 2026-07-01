using intsAPI.DTOs;

namespace intsAPI.Services
{
    public interface ICarService
    {
        Task<List<CarDto>> GetAllAsync();
        Task<CarDto?> GetByIdAsync(int id);
        Task<CarDto> CreateAsync(CreateCarRequest request);
        Task<bool> UpdateAsync(int id, UpdateCarRequest request);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
