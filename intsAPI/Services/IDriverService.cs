using intsAPI.DTOs;

namespace intsAPI.Services
{
    public class IDriverService
    {
        Task<List<DriverDto>> GetAllAsync();
        Task<DriverDto?> GetByIdAsync(int id);
        Task<DriverDto> CreateAsync(CreateDriverRequest request);
        Task<bool> UpdateAsync(int id, UpdateDriverRequest request);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
