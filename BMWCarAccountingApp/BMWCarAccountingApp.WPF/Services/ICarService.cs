using BMWCarAccountingApp.WPF.Models;
using System.Collections.ObjectModel;
namespace BMWCarAccountingApp.WPF.Services
{
    public interface ICarService
    {
        Task<List<Car>> GetAllCarsAsync();
        Task AddCarAsync(Car car);
        Task UpdateCarAsync(Car car);
        Task DeleteCarAsync(int id);
        Task SaveCarsAsync(IEnumerable<Car> cars);
    }
}
