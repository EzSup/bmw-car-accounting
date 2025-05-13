using BMWCarAccountingApp.Core.Data;
using BMWCarAccountingApp.Core.Services;
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
            return services;
        }
    }
}
