using BMWCarAccountingApp.Core.Models;
namespace BMWCarAccountingApp.Core.Services
{
    public interface ICarService
    {
        Task<List<Car>> GetAllCarsAsync();
        Task<Car> GetCarByIdAsync(int id);
        Task AddCarAsync(Car car);
        Task UpdateCarAsync(Car car);
        Task DeleteCarAsync(int id);
    }
}
