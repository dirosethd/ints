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
    public class FuelTypesViewModel : BaseViewModel
    {
        private readonly IDbService _db;

        public FuelTypesViewModel(IDbService db)
        {
            _db = db;

            RefreshCommand = new RelayCommand(async () => await LoadAsync());
            AddCommand = new RelayCommand(async () => await AddAsync(), CanAdd);
            DeleteSelectedCommand = new RelayCommand(async () => await DeleteSelectedAsync(), () => SelectedFuelType != null);

            _ = InitAsync();
        }

        public ObservableCollection<FuelType> FuelTypes { get; } = new();

        private FuelType? _selectedFuelType;
        public FuelType? SelectedFuelType
        {
            get => _selectedFuelType;
            set
            {
                if (Set(ref _selectedFuelType, value))
                    DeleteSelectedCommand.RaiseCanExecuteChanged();
            }
        }

        private string _newName = "";
        public string NewName
        {
            get => _newName;
            set
            {
                if (Set(ref _newName, value))
                    AddCommand.RaiseCanExecuteChanged();
            }
        }

        private decimal _newPricePerLiter = 0;
        public decimal NewPricePerLiter
        {
            get => _newPricePerLiter;
            set
            {
                if (Set(ref _newPricePerLiter, value))
                    AddCommand.RaiseCanExecuteChanged();
            }
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
            FuelTypes.Clear();
            foreach (var f in await _db.GetFuelTypesAsync()) FuelTypes.Add(f);
        }

        private bool CanAdd()
            => !string.IsNullOrWhiteSpace(NewName) && NewPricePerLiter > 0;

        private async Task AddAsync()
        {
            await _db.AddFuelTypeAsync(new FuelType
            {
                Name = NewName.Trim(),
                PricePerLiter = NewPricePerLiter
            });

            NewName = "";
            NewPricePerLiter = 0;

            await LoadAsync();
        }

        private async Task DeleteSelectedAsync()
        {
            if (SelectedFuelType == null) return;
            await _db.DeleteFuelTypeAsync(SelectedFuelType.Id);
            await LoadAsync();
        }
    }
}