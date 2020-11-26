using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApi.Context;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly WeatherContext _context;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, WeatherContext context)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("weatherreading")]
        public async Task<HttpStatusCode> WeatherReading(WeatherReading reading)
        {
            var place = reading.Place;
            
            var dbPlace = _context.Place.FirstOrDefault(p => p.Name == place.Name);

            if (dbPlace == null)
            {
                await _context.Place.AddAsync(place);
                await _context.SaveChangesAsync();
                dbPlace = await _context.Place.FirstOrDefaultAsync(w => w.Lat == place.Lat && w.Long == place.Long);
            }

            reading.Place = null;
            reading.PlaceId = dbPlace.Id;
            await _context.WeatherReading.AddAsync(reading);
            try
            {
                await _context.SaveChangesAsync();
                return HttpStatusCode.Accepted;
            }
            catch (Exception e)
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        [HttpGet("weatherreading")]
        public async Task<IEnumerable<WeatherReading>> WeatherReading()
        {
            return await _context.WeatherReading.OrderByDescending(w => w.Date).Take(3).ToListAsync();
        }

        [HttpGet("weatherreadingbydate")]
        public async Task<IEnumerable<WeatherReading>> WeatherReadingByDate(DateTime date)
        {
            return await _context.WeatherReading.Where(w => w.Date == date).ToListAsync();
        }

        [HttpGet("weatherreadingstartenddate")]
        public async Task<IEnumerable<WeatherReading>> WeatherReadingStartEndDate(DateTime startDate, DateTime endDate)
        {
            return await _context.WeatherReading.Where(w => w.Date >= startDate && w.Date <= endDate)
                .ToListAsync();
        }
    }
}
