
using BMWCarAccountingApp.WPF.Data;
using BMWCarAccountingApp.WPF.Models;
using Microsoft.EntityFrameworkCore;
namespace BMWCarAccountingApp.WPF.Services
{
    public class CarService : ICarService
    {
        private readonly AppDbContext _context;

        public CarService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Car>> GetAllCarsAsync()
        {
            return await _context.Cars.ToListAsync();
        }

        public async Task<Car> GetCarByIdAsync(int id)
        {
            return await _context.Cars.FindAsync(id);
        }

        public async Task AddCarAsync(Car car)
        {
            car.DateAdded = DateTime.Now;
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCarAsync(Car car)
        {
            var existingCar = _context.Cars.FirstOrDefault(c => c.Id == car.Id);
            if (existingCar != null)
            {
                existingCar.Model = car.Model;
                existingCar.Year = car.Year;
                existingCar.VIN = car.VIN;
                existingCar.Color = car.Color;
                existingCar.Price = car.Price;
            }
            await Task.CompletedTask;
        }

        public async Task DeleteCarAsync(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SaveCarsAsync(IEnumerable<Car> cars)
        {
            if (cars == null)
                throw new ArgumentNullException(nameof(cars));

            await Task.Delay(100); 
            foreach (var car in cars)
            {
                await UpdateCarAsync(car);
            }
        }
    }
}
