using System.Collections.Generic;

namespace NGK3.Models
{
    public class Place
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }

        public List<WeatherReading> WeatherReadings { get; set; }
    }
}