using BMWCarAccountingApp.WPF.Data;
using BMWCarAccountingApp.WPF.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace BMWCarAccountingApp.Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options => 
                                                    options.UseInMemoryDatabase("BMWDb"));
            services.AddScoped<ICarService, CarService>();
            services.AddScoped<IExportService, ExportService>();
            return services;
        }
    }
}
