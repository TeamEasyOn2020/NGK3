using System;

namespace NGK3.Models
{
    public class WeatherReadingDto
    {
        public DateTime Date { get; set; }
        public float TemperatureC { get; set; }
        public int Humidity { get; set; }
        public float AirPressure { get; set; }
        public PlaceDto Place { get; set; }

        public WeatherReadingDto(WeatherReading reading)
        {
            Date = reading.Date;
            TemperatureC = reading.TemperatureC;
            Humidity = reading.Humidity;
            AirPressure = reading.AirPressure;
            Place = new PlaceDto() {Lat = reading.Place.Lat, Long = reading.Place.Long, Name = reading.Place.Name};
        }
    }

}