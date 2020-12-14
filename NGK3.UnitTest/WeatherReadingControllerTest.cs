using NUnit.Framework;
using NGK3.Controllers;
using WebApi.Context;
using NSubstitute;
using NGK3.Hubs;
using Microsoft.AspNet.SignalR;
using NGK3.Models;
using System;
using System.Net;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace NGK3.UnitTest
{
    [TestFixture]
    public class WeatherReadingControllerTest
    {
        WeatherContext DbContext;
        WeatherForecastController uut;



        [SetUp]
        public void Setup()
        {
            DbContext = new WeatherContext();
            uut = new WeatherForecastController(DbContext, null);
        }

        [TearDown]
        public void TearDown()
        {
            DbContext.WeatherReading.RemoveRange(DbContext.WeatherReading);
            DbContext.Place.RemoveRange(DbContext.Place);
            DbContext.User.RemoveRange(DbContext.User);
            DbContext.SaveChanges();
        }

        [Test]
        public async Task GetAllWeatherReadings()
        {
            var reading = new WeatherReading
            {
                Date = DateTime.Now,
                TemperatureC = 10,
                Humidity = 100,
                AirPressure = 1000,
                Place = new Place { Lat = 58, Long = 10, Name = "Aarhus" }
            };

            DbContext.WeatherReading.Add(reading);
            DbContext.SaveChanges();

            var result = await uut.WeatherReading();

            Assert.IsInstanceOf<IEnumerable<WeatherReadingDto>>(result);
        }
        [Test]
        public async Task GetAllWeatherReadingsOnDate()
        {
            var reading = new WeatherReading
            {
                Date = DateTime.Now,
                TemperatureC = 10,
                Humidity = 100,
                AirPressure = 1000,
                Place = new Place { Lat = 58, Long = 10, Name = "Aarhus" }
            };

            DbContext.WeatherReading.Add(reading);
            DbContext.SaveChanges();

            var result = await uut.WeatherReadingByDate(DateTime.Now.Date);

            Assert.IsInstanceOf<IEnumerable<WeatherReadingDto>>(result);
        }
        [Test]
        public async Task GetAllWeatherReadingsOnDateReturnsOnlyOne()
        {
            var reading = new WeatherReading
            {
                Date = DateTime.Now,
                TemperatureC = 10,
                Humidity = 100,
                AirPressure = 1000,
                Place = new Place { Lat = 58, Long = 10, Name = "Aarhus" }
            };
            var reading2 = new WeatherReading
            {
                Date = DateTime.Now.AddDays(-2),
                TemperatureC = 10,
                Humidity = 100,
                AirPressure = 1000,
                Place = new Place { Lat = 58, Long = 10, Name = "Aarhus V" }
            };

            DbContext.WeatherReading.Add(reading);
            DbContext.WeatherReading.Add(reading2);
            DbContext.SaveChanges();

            var result = await uut.WeatherReadingByDate(DateTime.Now.Date);

            Assert.True(result.Count() == 1);
        }




    }
}
