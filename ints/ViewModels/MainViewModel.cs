using ints.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ints.ViewModels
{

    
        public class MainViewModel : BaseViewModel
        {
            private readonly INavigationService _nav;

            public MainViewModel(INavigationService nav)
            {
                _nav = nav ?? throw new ArgumentNullException(nameof(nav));

                _nav.CurrentChanged += () => OnPropertyChanged(nameof(CurrentView));

                NavigateToVehiclesCommand = new RelayCommand(() => _nav.NavigateTo<CarsViewModel>());
                NavigateToDriversCommand = new RelayCommand(() => _nav.NavigateTo<DriversViewModel>());
                NavigateToFuelTypesCommand = new RelayCommand(() => _nav.NavigateTo<FuelTypesViewModel>());
                NavigateToShipmentsCommand = new RelayCommand(() => _nav.NavigateTo<ShipmentsViewModel>());

                ExitCommand = new RelayCommand(() => App.Current.Shutdown());

                // стартовый экран
                _nav.NavigateTo<CarsViewModel>();
            }

            public BaseViewModel CurrentView => _nav.CurrentViewModel;

            public RelayCommand NavigateToVehiclesCommand { get; }
            public RelayCommand NavigateToDriversCommand { get; }
            public RelayCommand NavigateToFuelTypesCommand { get; }
            public RelayCommand NavigateToShipmentsCommand { get; }

            public RelayCommand ExitCommand { get; }
        }
    }

