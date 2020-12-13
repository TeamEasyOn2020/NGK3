using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NGK3.Hubs;
using NGK3.Models;
using WebApi.Context;

namespace NGK3.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly WeatherContext _context;
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IHubContext<ForecastHub> _hub;


        public WeatherForecastController(ILogger<WeatherForecastController> logger, WeatherContext context, IHubContext<ForecastHub> hub)
        {
            _context = context;
            _hub = hub;
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
                await SendMessage(reading);
                return HttpStatusCode.Accepted;
            }
            catch (Exception e)
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        [HttpGet("weatherreading")]
        public async Task<IEnumerable<WeatherReadingDto>> WeatherReading()
        {
            
            var readings = await _context.WeatherReading.OrderByDescending(w => w.Date).Include(w => w.Place).Take(3).ToListAsync();


            return readings.Select(weatherReading => new WeatherReadingDto(weatherReading)).ToList();
        }

        [HttpGet("weatherreading/{date}")]
        public async Task<IEnumerable<WeatherReadingDto>> WeatherReadingByDate([FromRoute] DateTime date)
        {
            var readings = await _context.WeatherReading.Where(w => w.Date.Date == date.Date).Include(w => w.Place).OrderBy(w => w.Date).ToListAsync();
            return readings.Select(weatherReading => new WeatherReadingDto(weatherReading)).ToList();
        }

        [HttpGet("weatherreading/{startDate}/{endDate}")]
        public async Task<IEnumerable<WeatherReadingDto>> WeatherReadingStartEndDate([FromRoute] DateTime startDate, [FromRoute] DateTime endDate)
        {
           var readings = await _context.WeatherReading.Where(w => w.Date.Date >= startDate.Date && w.Date.Date <= endDate.Date).Include(w => w.Place).OrderBy(w => w.Date).ToListAsync();
            return readings.Select(weatherReading => new WeatherReadingDto(weatherReading)).ToList();
        }

        public async Task SendMessage(WeatherReading reading)
        {
            var dtoReading = new WeatherReadingDto(reading);
            var jcontent = Newtonsoft.Json.JsonConvert.SerializeObject(dtoReading);
            await _hub.Clients.All.SendAsync("newForecast", jcontent);
        }
    }
}
