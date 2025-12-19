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
    public class CarsViewModel : BaseViewModel
    {
        private readonly IDbService _db;

        public CarsViewModel(IDbService db)
        {
            _db = db;

            RefreshCommand = new RelayCommand(async () => await LoadAsync());
            AddCommand = new RelayCommand(async () => await AddAsync(), CanAdd);
            DeleteSelectedCommand = new RelayCommand(async () => await DeleteSelectedAsync(), () => SelectedCar != null);

            _ = InitAsync();
        }

        public ObservableCollection<Car> Cars { get; } = new();

        private Car? _selectedCar;
        public Car? SelectedCar
        {
            get => _selectedCar;
            set
            {
                if (Set(ref _selectedCar, value))
                    DeleteSelectedCommand.RaiseCanExecuteChanged();
            }
        }

        private string _newRegNumber = "";
        public string NewRegNumber
        {
            get => _newRegNumber;
            set
            {
                if (Set(ref _newRegNumber, value))
                    AddCommand.RaiseCanExecuteChanged();
            }
        }

        private string _newBrand = "";
        public string NewBrand
        {
            get => _newBrand;
            set => Set(ref _newBrand, value);
        }

        private string _newModel = "";
        public string NewModel
        {
            get => _newModel;
            set => Set(ref _newModel, value);
        }

        private int _newYear = DateTime.Now.Year;
        public int NewYear
        {
            get => _newYear;
            set => Set(ref _newYear, value);
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
            Cars.Clear();
            var cars = await _db.GetCarsAsync();
            foreach (var c in cars) Cars.Add(c);
        }

        private bool CanAdd() => !string.IsNullOrWhiteSpace(NewRegNumber);

        private async Task AddAsync()
        {
            await _db.AddCarAsync(new Car
            {
                RegNumber = NewRegNumber.Trim(),
                Brand = NewBrand.Trim(),
                Model = NewModel.Trim(),
                Year = NewYear
            });

            NewRegNumber = "";
            NewBrand = "";
            NewModel = "";
            NewYear = DateTime.Now.Year;

            await LoadAsync();
        }

        private async Task DeleteSelectedAsync()
        {
            if (SelectedCar == null) return;
            await _db.DeleteCarAsync(SelectedCar.Id);
            await LoadAsync();
        }
    }
}