using BMWCarAccountingApp.WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMWCarAccountingApp.WPF.Services
{
    public interface IExportService
    {
        Task ExportToExcelAsync(IEnumerable<Car> cars, string filePath);
        Task ExportToCsvAsync(IEnumerable<Car> cars, string filePath);
        Task<(IList<Car> ImportedCars, IList<string> Errors)> ImportFromCsvAsync(string filePath, IEnumerable<Car> existingCars);
    }
}
