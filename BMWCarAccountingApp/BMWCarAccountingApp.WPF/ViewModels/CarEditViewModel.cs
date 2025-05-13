
using BMWCarAccountingApp.WPF.Models;
using BMWCarAccountingApp.WPF.Navigation;
using BMWCarAccountingApp.WPF.Services;
using BMWCarAccountingApp.WPF.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
namespace BMWCarAccountingApp.WPF.ViewModels
{
    public partial class CarCreateViewModel : ObservableObject
    {
        private readonly ICarService _carService;
        private readonly NavigationService _navigationService;
        private readonly FrameworkElement _view;

        [ObservableProperty]
        private Car _car = new Car();

        public CarCreateViewModel()
        {
            
        }

        public CarCreateViewModel(ICarService carService, NavigationService navigationService, FrameworkElement view = null)
        {
            _carService = carService;
            _navigationService = navigationService;
            _view = view;
        }

        [RelayCommand]
        private async void SaveCar()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Car.Model) || string.IsNullOrWhiteSpace(Car.VIN))
                {
                    MessageBox.Show("Модель і VIN-код є обов’язковими.", "Помилка валідації");
                    return;
                }
                await _carService.AddCarAsync(Car);
                _navigationService.NavigateTo<CarListPage, CarListViewModel>();
                if (_view != null)
                {
                    Window.GetWindow(_view)?.Close();
                }
                else
                {
                    Application.Current.Windows.OfType<AdditionalWindow>().FirstOrDefault()?.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка збереження: {ex.Message}", "Помилка");
            }
        }
    }
}
