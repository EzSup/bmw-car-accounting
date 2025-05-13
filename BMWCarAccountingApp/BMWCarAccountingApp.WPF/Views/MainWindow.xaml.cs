using BMWCarAccountingApp.WPF.ViewModels;
using BMWCarAccountingApp.WPF.Views;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NavigationService = BMWCarAccountingApp.WPF.Navigation.NavigationService;

namespace BMWCarAccountingApp.WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel, NavigationService navigationService)
    {
        this.DataContext = viewModel;
        InitializeComponent();
        navigationService.SetFrame(MainFrame); 
        navigationService.NavigateTo<CarListPage, CarListViewModel>();
    }
}
