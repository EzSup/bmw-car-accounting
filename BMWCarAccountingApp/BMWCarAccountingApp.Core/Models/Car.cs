namespace BMWCarAccountingApp.Core.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string Model { get; set; } 
        public int Year { get; set; }
        public string VIN { get; set; } 
        public string Color { get; set; }
        public decimal Price { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
