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
    public class DriversViewModel : BaseViewModel
    {
        private readonly IDbService _db;

        public DriversViewModel(IDbService db)
        {
            _db = db;

            RefreshCommand = new RelayCommand(async () => await LoadAsync());
            AddCommand = new RelayCommand(async () => await AddAsync(), CanAdd);
            DeleteSelectedCommand = new RelayCommand(async () => await DeleteSelectedAsync(), () => SelectedDriver != null);

            _ = InitAsync();
        }

        public ObservableCollection<Driver> Drivers { get; } = new();
        public ObservableCollection<Car> Cars { get; } = new();

        private Driver? _selectedDriver;
        public Driver? SelectedDriver
        {
            get => _selectedDriver;
            set
            {
                if (Set(ref _selectedDriver, value))
                    DeleteSelectedCommand.RaiseCanExecuteChanged();
            }
        }

        private string _newFullName = "";
        public string NewFullName
        {
            get => _newFullName;
            set
            {
                if (Set(ref _newFullName, value))
                    AddCommand.RaiseCanExecuteChanged();
            }
        }

        private string _newLicenseNumber = "";
        public string NewLicenseNumber
        {
            get => _newLicenseNumber;
            set => Set(ref _newLicenseNumber, value);
        }

        private Car? _selectedCarForNewDriver;
        public Car? SelectedCarForNewDriver
        {
            get => _selectedCarForNewDriver;
            set
            {
                if (Set(ref _selectedCarForNewDriver, value))
                    AddCommand.RaiseCanExecuteChanged();
            }
        }

        public DateTime? NewHireDate { get; set; } = DateTime.Today;

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
            Cars.Clear();
            foreach (var c in await _db.GetCarsAsync()) Cars.Add(c);

            Drivers.Clear();
            foreach (var d in await _db.GetDriversAsync()) Drivers.Add(d);
        }

        private bool CanAdd()
            => !string.IsNullOrWhiteSpace(NewFullName) && SelectedCarForNewDriver != null;

        private async Task AddAsync()
        {
            if (SelectedCarForNewDriver == null) return;

            await _db.AddDriverAsync(new Driver
            {
                FullName = NewFullName.Trim(),
                LicenseNumber = NewLicenseNumber.Trim(),
                HireDate = DateOnly.FromDateTime(NewHireDate ?? DateTime.Today),
                CarId = SelectedCarForNewDriver.Id
            });

            NewFullName = "";
            NewLicenseNumber = "";
            SelectedCarForNewDriver = null;

            await LoadAsync();
        }

        private async Task DeleteSelectedAsync()
        {
            if (SelectedDriver == null) return;
            await _db.DeleteDriverAsync(SelectedDriver.Id);
            await LoadAsync();
        }
    }
}