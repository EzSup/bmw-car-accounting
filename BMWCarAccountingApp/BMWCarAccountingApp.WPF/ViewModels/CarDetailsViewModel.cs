using BMWCarAccountingApp.WPF.Models;
using BMWCarAccountingApp.WPF.Services;
using BMWCarAccountingApp.WPF.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace BMWCarAccountingApp.WPF.ViewModels
{
    public partial class CarDetailsViewModel : ObservableObject
    {
        private readonly ICarService _carService;
        private readonly FrameworkElement _view;

        [ObservableProperty]
        private Car _car;

        public CarDetailsViewModel(ICarService carService, Car car, FrameworkElement view = null)
        {
            _carService = carService;
            _car = car;
            _view = view;
        }

        [RelayCommand]
        private void Close()
        {
            if (_view != null)
            {
                Window.GetWindow(_view)?.Close();
            }
            else
            {
                Application.Current.Windows.OfType<AdditionalWindow>().FirstOrDefault()?.Close();
            }
        }
    }
}
