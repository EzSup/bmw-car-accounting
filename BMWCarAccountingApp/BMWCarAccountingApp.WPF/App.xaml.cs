using BMWCarAccountingApp.Core;
using BMWCarAccountingApp.WPF.Data;
using BMWCarAccountingApp.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;
using System.Configuration;
using System.Data;
using System.Windows;
using NavigationService = BMWCarAccountingApp.WPF.Navigation.NavigationService;

namespace BMWCarAccountingApp.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public IServiceProvider ServiceProvider { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        ExcelPackage.License.SetNonCommercialPersonal("VTC_Project");
        
        var services = new ServiceCollection();
        services.AddCoreServices(); 
        services.AddSingleton<NavigationService>();
        services.AddSingleton<MainViewModel>();
        services.AddTransient<CarListViewModel>();
        services.AddTransient<CarCreateViewModel>();
        services.AddTransient<CarDetailsViewModel>();
        ServiceProvider = services.BuildServiceProvider();
        
        using (var scope = ServiceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetService<AppDbContext>();
            if (dbContext == null)
            {
                throw new InvalidOperationException("AppDbContext is not registered in DI.");
            }
            dbContext.Seed();
        }

        var mainWindow = new MainWindow(
            ServiceProvider.GetService<MainViewModel>()!, 
            ServiceProvider.GetService<NavigationService>()!);
        mainWindow.Show();
    }
}
