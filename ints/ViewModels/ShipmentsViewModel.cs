using ints.Models;
using ints.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ints.ViewModels
{
    public class ShipmentsViewModel : BaseViewModel
    {
        private readonly IDbService _db;

        public ShipmentsViewModel(IDbService db)
        {
            _db = db;

            RefreshCommand = new RelayCommand(async () => await LoadAsync());
            AddCommand = new RelayCommand(async () => await AddAsync(), CanAdd);
            DeleteSelectedCommand = new RelayCommand(async () => await DeleteSelectedAsync(), () => SelectedShipment != null);

            _ = InitAsync();
        }

        public ObservableCollection<Shipment> Shipments { get; } = new();
        public ObservableCollection<Car> Cars { get; } = new();
        public ObservableCollection<Driver> DriversAll { get; } = new();
        public ObservableCollection<Driver> DriversFiltered { get; } = new();
        public ObservableCollection<FuelType> FuelTypes { get; } = new();

        private Shipment? _selectedShipment;
        public Shipment? SelectedShipment
        {
            get => _selectedShipment;
            set
            {
                if (Set(ref _selectedShipment, value))
                    DeleteSelectedCommand.RaiseCanExecuteChanged();
            }
        }

        private DateTime _newDate = DateTime.Today;
        public DateTime NewDate
        {
            get => _newDate;
            set => Set(ref _newDate, value);
        }

        private string _newFrom = "";
        public string NewFrom
        {
            get => _newFrom;
            set
            {
                if (Set(ref _newFrom, value))
                    AddCommand.RaiseCanExecuteChanged();
            }
        }

        private string _newTo = "";
        public string NewTo
        {
            get => _newTo;
            set
            {
                if (Set(ref _newTo, value))
                    AddCommand.RaiseCanExecuteChanged();
            }
        }

        private decimal _newDistanceKm;
        public decimal NewDistanceKm
        {
            get => _newDistanceKm;
            set
            {
                if (Set(ref _newDistanceKm, value))
                    AddCommand.RaiseCanExecuteChanged();
            }
        }

        private decimal _newVolumeLiters;
        public decimal NewVolumeLiters
        {
            get => _newVolumeLiters;
            set
            {
                if (Set(ref _newVolumeLiters, value))
                    AddCommand.RaiseCanExecuteChanged();
            }
        }

        private Car? _selectedCar;
        public Car? SelectedCar
        {
            get => _selectedCar;
            set
            {
                if (Set(ref _selectedCar, value))
                {
                    RebuildDriversFiltered();
                    SelectedDriver = null;
                    AddCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private Driver? _selectedDriver;
        public Driver? SelectedDriver
        {
            get => _selectedDriver;
            set
            {
                if (Set(ref _selectedDriver, value))
                    AddCommand.RaiseCanExecuteChanged();
            }
        }

        private FuelType? _selectedFuelType;
        public FuelType? SelectedFuelType
        {
            get => _selectedFuelType;
            set
            {
                if (Set(ref _selectedFuelType, value))
                    AddCommand.RaiseCanExecuteChanged();
            }
        }

        private string _error = "";
        public string Error
        {
            get => _error;
            set => Set(ref _error, value);
        }

        public RelayCommand RefreshCommand { get; }
        public RelayCommand AddCommand { get; }
        public RelayCommand DeleteSelectedCommand { get; }

        private async Task InitAsync()
        {
            await _db.EnsureCreatedAsync();
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            Error = "";

            Cars.Clear();
            foreach (var c in await _db.GetCarsAsync()) Cars.Add(c);

            DriversAll.Clear();
            foreach (var d in await _db.GetDriversAsync()) DriversAll.Add(d);

            FuelTypes.Clear();
            foreach (var f in await _db.GetFuelTypesAsync()) FuelTypes.Add(f);

            RebuildDriversFiltered();

            Shipments.Clear();
            foreach (var s in await _db.GetShipmentsAsync()) Shipments.Add(s);
        }

        private void RebuildDriversFiltered()
        {
            DriversFiltered.Clear();
            if (SelectedCar == null) return;

            foreach (var d in DriversAll.Where(x => x.CarId == SelectedCar.Id))
                DriversFiltered.Add(d);
        }

        private bool CanAdd()
        {
            if (SelectedCar == null) return false;
            if (SelectedDriver == null) return false;
            if (SelectedFuelType == null) return false;

            if (string.IsNullOrWhiteSpace(NewFrom)) return false;
            if (string.IsNullOrWhiteSpace(NewTo)) return false;
            if (NewDistanceKm <= 0) return false;
            if (NewVolumeLiters <= 0) return false;

            // доп. проверка по условию задания:
            if (SelectedDriver.CarId != SelectedCar.Id) return false;

            return true;
        }

        private async Task AddAsync()
        {
            try
            {
                Error = "";

                if (SelectedCar == null || SelectedDriver == null || SelectedFuelType == null) return;

                if (SelectedDriver.CarId != SelectedCar.Id)
                    throw new InvalidOperationException("Выбранный водитель закреплён за другим автомобилем.");

                await _db.AddShipmentAsync(new Shipment
                {
                    Date = DateOnly.FromDateTime(NewDate),   // было Date = NewDate
                    FromLocation = NewFrom.Trim(),           // было From =
                    ToLocation = NewTo.Trim(),               // было To =
                    DistanceKm = NewDistanceKm,              // теперь decimal
                    VolumeLiters = NewVolumeLiters,
                    CarId = SelectedCar.Id,
                    DriverId = SelectedDriver.Id,
                    FuelTypeId = SelectedFuelType.Id
                });

                // очистка
                NewFrom = "";
                NewTo = "";
                NewDistanceKm = 0;
                NewVolumeLiters = 0;

                await LoadAsync();
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }
        }

        private async Task DeleteSelectedAsync()
        {
            if (SelectedShipment == null) return;
            await _db.DeleteShipmentAsync(SelectedShipment.Id);
            await LoadAsync();
        }
    }
}
