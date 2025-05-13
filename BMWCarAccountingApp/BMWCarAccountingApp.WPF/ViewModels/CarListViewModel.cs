// BMWCarAccountingApp.WPF/ViewModels/CarListViewModel.cs
using BMWCarAccountingApp.WPF.Models;
using BMWCarAccountingApp.WPF.Navigation;
using BMWCarAccountingApp.WPF.Services;
using BMWCarAccountingApp.WPF.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace BMWCarAccountingApp.WPF.ViewModels
{
    public partial class CarListViewModel : ObservableObject
    {
        private readonly ICarService _carService;
        private readonly IExportService _exportService;
        private readonly NavigationService _navigationService;
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private ObservableCollection<Car> _cars = new();

        [ObservableProperty]
        private Car _selectedCar;

        [ObservableProperty]
        private bool _isCarSelected;

        public CarListViewModel(ICarService carService, IExportService exportService, NavigationService navigationService, IServiceProvider serviceProvider)
        {
            _carService = carService;
            _exportService = exportService;
            _navigationService = navigationService;
            _serviceProvider = serviceProvider;
            SetLanguage("uk-UA");
            LoadCarsAsync();
        }

        partial void OnSelectedCarChanged(Car value)
        {
            IsCarSelected = value != null;
        }

        [RelayCommand]
        private void NavigateToAddCar()
        {
            var carEditViewModel = _serviceProvider.GetService<CarCreateViewModel>();
            var carEditPage = new CarEditPage { DataContext = carEditViewModel };

            var carEditWindow = new AdditionalWindow(carEditPage)
            {
                Owner = Application.Current.MainWindow
            };

            carEditWindow.Closed += async (s, e) =>
            {
                await LoadCarsAsync();
            };

            carEditWindow.Show();
        }

        [RelayCommand]
        private async void DeleteCar()
        {
            if (SelectedCar == null)
            {
                MessageBox.Show((string)Application.Current.Resources["ErrorNoCarSelectedDelete"],
                                (string)Application.Current.Resources["ErrorTitle"],
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(string.Format((string)Application.Current.Resources["ConfirmDeleteCar"], SelectedCar.Model),
                                         (string)Application.Current.Resources["ConfirmDeleteTitle"],
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _carService.DeleteCarAsync(SelectedCar.Id);
                    await LoadCarsAsync();
                    MessageBox.Show((string)Application.Current.Resources["SuccessCarDeleted"],
                                    (string)Application.Current.Resources["SuccessTitle"],
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format((string)Application.Current.Resources["ErrorDeleteCar"], ex.Message),
                                    (string)Application.Current.Resources["ErrorTitle"],
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private void NavigateToDetails()
        {
            if (SelectedCar == null)
            {
                MessageBox.Show((string)Application.Current.Resources["ErrorNoCarSelectedDetails"],
                                (string)Application.Current.Resources["ErrorTitle"],
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var carDetailsPage = new CarDetailsPage();
            var carDetailsViewModel = new CarDetailsViewModel(_serviceProvider.GetService<ICarService>(), SelectedCar);
            carDetailsPage.DataContext = carDetailsViewModel;

            var additionalWindow = new AdditionalWindow(carDetailsPage)
            {
                Owner = Application.Current.MainWindow,
                DataContext = new { WindowTitle = string.Format((string)Application.Current.Resources["DetailsWindowTitle"], SelectedCar.Model) }
            };

            additionalWindow.Show();
        }

        [RelayCommand]
        private void ChangeLanguage(string lang)
        {
            if (string.IsNullOrEmpty(lang))
            {
                MessageBox.Show((string)Application.Current.Resources["ErrorInvalidLanguage"],
                                (string)Application.Current.Resources["ErrorTitle"],
                                MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                SetLanguage(lang);
                Application.Current.MainWindow?.InvalidateVisual();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format((string)Application.Current.Resources["ErrorChangeLanguage"], ex.Message),
                                (string)Application.Current.Resources["ErrorTitle"],
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetLanguage(string lang)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(lang);
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);

            Application.Current.Resources.Clear();
            ResourceDictionary resdict = new()
            {
                Source = new Uri($"Resources/Localization/Dictionary-{lang}.xaml", UriKind.Relative)
            };
            Application.Current.Resources.MergedDictionaries.Add(resdict);
        }

        [RelayCommand]
        private async void ResetChanges()
        {
            await LoadCarsAsync();
        }

        [RelayCommand]
        private async void SaveChanges()
        {
            try
            {
                await _carService.SaveCarsAsync(Cars);
                MessageBox.Show((string)Application.Current.Resources["SuccessSaveChanges"],
                                (string)Application.Current.Resources["SuccessTitle"]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format((string)Application.Current.Resources["ErrorSaveChanges"], ex.Message),
                                (string)Application.Current.Resources["ErrorTitle"]);
            }
        }

        [RelayCommand]
        private async void ExportToExcel()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                DefaultExt = "xlsx",
                FileName = "CarsExport.xlsx"
            };

            if (saveFileDialog.ShowDialog() != true)
                return;

            try
            {
                await _exportService.ExportToExcelAsync(Cars, saveFileDialog.FileName);
                MessageBox.Show((string)Application.Current.Resources["SuccessExportExcel"],
                                (string)Application.Current.Resources["SuccessTitle"],
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format((string)Application.Current.Resources["ErrorExportExcel"], ex.Message),
                                (string)Application.Current.Resources["ErrorTitle"],
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async void ExportToCsv()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                DefaultExt = "csv",
                FileName = "CarsExport.csv"
            };

            if (saveFileDialog.ShowDialog() != true)
                return;

            try
            {
                await _exportService.ExportToCsvAsync(Cars, saveFileDialog.FileName);
                MessageBox.Show((string)Application.Current.Resources["SuccessExportCsv"],
                                (string)Application.Current.Resources["SuccessTitle"],
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format((string)Application.Current.Resources["ErrorExportCsv"], ex.Message),
                                (string)Application.Current.Resources["ErrorTitle"],
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async void ImportFromCsv()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                DefaultExt = "csv",
                Title = (string)Application.Current.Resources["SelectCsvFileTitle"]
            };

            if (openFileDialog.ShowDialog() != true)
                return;

            try
            {
                var (importedCars, errors) = await _exportService.ImportFromCsvAsync(openFileDialog.FileName, Cars);

                if (importedCars.Any())
                {
                    foreach (var car in importedCars)
                    {
                        await _carService.AddCarAsync(car);
                    }
                    await LoadCarsAsync();
                    var message = string.Format((string)Application.Current.Resources["SuccessImportCsv"], importedCars.Count);
                    if (errors.Any())
                    {
                        message += $"\n{Application.Current.Resources["ErrorsLabel"]}:\n{string.Join("\n", errors)}";
                    }
                    MessageBox.Show(message, (string)Application.Current.Resources["ImportResultTitle"], MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show((string)Application.Current.Resources["ErrorNoCarsImported"] + "\n" +
                                    (errors.Any() ? $"{Application.Current.Resources["ErrorsLabel"]}:\n{string.Join("\n", errors)}" :
                                                    (string)Application.Current.Resources["ErrorNoDataToImport"]),
                                    (string)Application.Current.Resources["ErrorTitle"],
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format((string)Application.Current.Resources["ErrorImportCsv"], ex.Message),
                                (string)Application.Current.Resources["ErrorTitle"],
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadCarsAsync()
        {
            try
            {
                Cars = new ObservableCollection<Car>(await _carService.GetAllCarsAsync());
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format((string)Application.Current.Resources["ErrorLoadCars"], ex.Message),
                                (string)Application.Current.Resources["ErrorTitle"]);
            }
        }
    }
}