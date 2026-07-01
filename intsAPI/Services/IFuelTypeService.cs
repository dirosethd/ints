namespace intsAPI.Services
{
    public interface IFuelTypeService
    {
        Task<List<FuelTypeDto>> GetAllAsync();
        Task<FuelTypeDto?> GetByIdAsync(int id);
        Task<FuelTypeDto> CreateAsync(CreateFuelTypeRequest request);
        Task<bool> UpdateAsync(int id, UpdateFuelTypeRequest request);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
