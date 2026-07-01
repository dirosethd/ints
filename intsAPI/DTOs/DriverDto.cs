namespace intsAPI.DTOs
{
    public class DriverDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public DateOnly HireDate { get; set; }
        public int CarId { get; set; }
        public CarDto? Car { get; set; }
       
    }
}