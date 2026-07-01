namespace intsAPI.DTOs
{
    public class CarDto
    {
        public int Id { get; set; }
        public string RegNumber { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public List<DriverDto> Drivers { get; set; } = new();
    }
}
