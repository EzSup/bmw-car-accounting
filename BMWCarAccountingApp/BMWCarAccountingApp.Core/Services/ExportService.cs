using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMWCarAccountingApp.Core.Services
{
    public class ExportService : IExportService
    {
        public async Task ExportToExcelAsync(IEnumerable<Car> cars, string filePath)
        {
            if (cars == null || !cars.Any())
                throw new InvalidOperationException((string)Application.Current.Resources["ErrorNoDataToExport"]);

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Cars");

            worksheet.Cells[1, 1].Value = Application.Current.Resources["ModelColumnHeader"];
            worksheet.Cells[1, 2].Value = Application.Current.Resources["YearColumnHeader"];
            worksheet.Cells[1, 3].Value = Application.Current.Resources["VINColumnHeader"];
            worksheet.Cells[1, 4].Value = Application.Current.Resources["ColorLabel"];
            worksheet.Cells[1, 5].Value = Application.Current.Resources["PriceColumnHeader"];

            using (var range = worksheet.Cells[1, 1, 1, 5])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            int row = 2;
            foreach (var car in cars)
            {
                worksheet.Cells[row, 1].Value = car.Model;
                worksheet.Cells[row, 2].Value = car.Year;
                worksheet.Cells[row, 3].Value = car.VIN;
                worksheet.Cells[row, 4].Value = car.Color;
                worksheet.Cells[row, 5].Value = car.Price;
                worksheet.Cells[row, 5].Style.Numberformat.Format = "₴#,##0.00";
                row++;
            }

            worksheet.Cells.AutoFitColumns();

            await File.WriteAllBytesAsync(filePath, package.GetAsByteArray());
        }

        public async Task ExportToCsvAsync(IEnumerable<Car> cars, string filePath)
        {
            if (cars == null || !cars.Any())
                throw new InvalidOperationException((string)Application.Current.Resources["ErrorNoDataToExport"]);

            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine($"{Application.Current.Resources["ModelColumnHeader"]};{Application.Current.Resources["YearColumnHeader"]};{Application.Current.Resources["VINColumnHeader"]};{Application.Current.Resources["ColorLabel"]};{Application.Current.Resources["PriceColumnHeader"]}");

            foreach (var car in cars)
            {
                var model = $"\"{car.Model.Replace("\"", "\"\"")}\"";
                var vin = $"\"{car.VIN.Replace("\"", "\"\"")}\"";
                var color = $"\"{car.Color.Replace("\"", "\"\"")}\"";
                csvBuilder.AppendLine($"{model};{car.Year};{vin};{color};{car.Price:F2}");
            }

            await File.WriteAllTextAsync(filePath, csvBuilder.ToString(), new UTF8Encoding(true));
        }

        public async Task<(IList<Car> ImportedCars, IList<string> Errors)> ImportFromCsvAsync(string filePath, IEnumerable<Car> existingCars)
        {
            var lines = await File.ReadAllLinesAsync(filePath, Encoding.UTF8);
            if (lines.Length < 1)
                throw new InvalidOperationException((string)Application.Current.Resources["ErrorEmptyCsv"]);

            var header = lines[0].Trim();
            var expectedHeader = $"{Application.Current.Resources["ModelColumnHeader"]};{Application.Current.Resources["YearColumnHeader"]};{Application.Current.Resources["VINColumnHeader"]};{Application.Current.Resources["ColorLabel"]};{Application.Current.Resources["PriceColumnHeader"]}";
            if (header != expectedHeader)
                throw new InvalidOperationException(string.Format((string)Application.Current.Resources["ErrorInvalidCsvHeader"], expectedHeader));

            var importedCars = new List<Car>();
            var errors = new List<string>();

            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var values = line.Split(';').Select(v => v.Trim('"')).ToArray();
                if (values.Length != 5)
                {
                    errors.Add(string.Format((string)Application.Current.Resources["ErrorInvalidFieldCount"], i + 1));
                    continue;
                }

                try
                {
                    var car = new Car
                    {
                        Id = existingCars.Any() ? existingCars.Max(c => c.Id) + i : i,
                        Model = values[0],
                        Year = int.Parse(values[1]),
                        VIN = values[2],
                        Color = values[3],
                        Price = decimal.Parse(values[4], System.Globalization.CultureInfo.InvariantCulture)
                    };

                    if (string.IsNullOrWhiteSpace(car.Model) || string.IsNullOrWhiteSpace(car.VIN))
                    {
                        errors.Add(string.Format((string)Application.Current.Resources["ErrorEmptyModelOrVin"], i + 1));
                        continue;
                    }
                    if (car.Year < 1886 || car.Year > DateTime.Now.Year + 1)
                    {
                        errors.Add(string.Format((string)Application.Current.Resources["ErrorInvalidYear"], i + 1, car.Year));
                        continue;
                    }
                    if (car.Price < 0)
                    {
                        errors.Add(string.Format((string)Application.Current.Resources["ErrorNegativePrice"], i + 1, car.Price));
                        continue;
                    }

                    importedCars.Add(car);
                }
                catch (FormatException)
                {
                    errors.Add(string.Format((string)Application.Current.Resources["ErrorInvalidNumberFormat"], i + 1));
                }
                catch (Exception ex)
                {
                    errors.Add(string.Format((string)Application.Current.Resources["ErrorGeneral"], i + 1, ex.Message));
                }
            }

            return (importedCars, errors);
        }
    }
}
