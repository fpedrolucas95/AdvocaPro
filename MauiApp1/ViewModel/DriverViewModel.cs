using AdvocaPro.Models;
using AdvocaPro.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AdvocaPro.ViewModel
{
    public partial class DriverViewModel : ObservableObject
    {
        private readonly DriverService _driverService;
        private readonly PrioCardService _prioCardService;
        private readonly VehicleService _vehicleService;

        public DriverViewModel(DriverService driverService, PrioCardService prioCardService, VehicleService vehicleService)
        {
            _driverService = driverService;
            _prioCardService = prioCardService;
            _vehicleService = vehicleService;

            LoadData();
        }

        [ObservableProperty]
        private DriverModel _driver = new DriverModel();

        [ObservableProperty]
        private ObservableCollection<PrioCardModel> _prioCards;

        [ObservableProperty]
        private ObservableCollection<VehicleModel> _vehicles;

        [ObservableProperty]
        private bool _isAddDriverPageVisible = false;

        [ObservableProperty]
        private int _columnSize = 0;

        [RelayCommand]
        private async Task OpenAddDriverAsync()
        {
            IsAddDriverPageVisible = true;
            ColumnSize = 420;
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            // Implementar a lógica para salvar o motorista
            _driverService.AddDriver(Driver);
            IsAddDriverPageVisible = false;
            ColumnSize = 0;
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            IsAddDriverPageVisible = false;
            ColumnSize = 0;
        }

        private void LoadData()
        {
            PrioCards = new ObservableCollection<PrioCardModel>(_prioCardService.GetAvailablePrioCards());
            Vehicles = new ObservableCollection<VehicleModel>(_vehicleService.GetAvailableVehicles());
        }
    }
}
