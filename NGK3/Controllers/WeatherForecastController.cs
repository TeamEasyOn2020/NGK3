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
using Microsoft.AspNetCore.Authorization;

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
        [Authorize]
        [HttpPost("weatherreading")]
        public async Task<HttpStatusCode> WeatherReading([FromBody] WeatherReading reading)
        {
            try
            {
                var dbPlace = _context.Place.Where(p => p.Name == reading.Place.Name).FirstOrDefault();

                if (dbPlace == null)
                {
                    await _context.Place.AddAsync(reading.Place);
                    await _context.SaveChangesAsync();
                    dbPlace = await _context.Place.FirstOrDefaultAsync(w => w.Lat == reading.Place.Lat && w.Long == reading.Place.Long);
                }

                reading.Place = dbPlace;
                await _context.WeatherReading.AddAsync(reading);

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

        [HttpGet("weatherreading/{date}")]
        public async Task<IEnumerable<WeatherReading>> WeatherReadingByDate([FromRoute] DateTime date)
        {
            return await _context.WeatherReading.Where(w => w.Date.Date == date.Date).OrderBy(w => w.Date).ToListAsync();
        }

        [HttpGet("weatherreading/{startDate}/{endDate}")]
        public async Task<IEnumerable<WeatherReading>> WeatherReadingStartEndDate([FromRoute] DateTime startDate, [FromRoute] DateTime endDate)
        {
            return await _context.WeatherReading.Where(w => w.Date >= startDate && w.Date <= endDate).OrderBy(w => w.Date)
                .ToListAsync();
        }
    }
}
