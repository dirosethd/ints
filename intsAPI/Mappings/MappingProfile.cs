using AutoMapper;
using ints.Models;
using intsAPI.DTOs;
using intsAPI.Help;
using intsAPI.Services;

namespace intsAPI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Car mappings
            CreateMap<Car, CarDto>();
            CreateMap<CreateCarRequest, Car>();
            CreateMap<UpdateCarRequest, Car>();

            // Driver mappings
            CreateMap<Driver, DriverDto>();
            CreateMap<CreateDriverRequest, Driver>();
            CreateMap<UpdateDriverRequest, Driver>();

            // FuelType mappings
            CreateMap<FuelType, FuelTypeDto>();
            CreateMap<CreateFuelTypeRequest, FuelType>();
            CreateMap<UpdateFuelTypeRequest, FuelType>();

            // Shipment mappings
            CreateMap<Shipment, ShipmentDto>();
            CreateMap<CreateShipmentRequest, Shipment>();
            CreateMap<UpdateShipmentRequest, Shipment>();
        }
    }
}

