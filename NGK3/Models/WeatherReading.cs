using System;

namespace NGK3.Models
{
    public class WeatherReading
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public float TemperatureC { get; set; }
        public int Humidity { get; set; }
        public float AirPressure { get; set; }
        public Place Place { get; set; }
        public int PlaceId { get; set; }
    }
}
