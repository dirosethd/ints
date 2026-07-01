using intsAPI.DTOs;

namespace intsAPI.Services
{
    public interface IShipmentService
    {
        Task<List<ShipmentDto>> GetAllAsync();
        Task<ShipmentDto?> GetByIdAsync(int id);
        Task<ShipmentDto> CreateAsync(CreateShipmentRequest request);
        Task<bool> UpdateAsync(int id, UpdateShipmentRequest request);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
