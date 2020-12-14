using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using NGK3.Models;
using WebApi.Context;

namespace NGK3.Hubs
{
    
    public class ForecastHub : Hub
    {
        public ForecastHub(){}
        [HubMethodName("newForecast")]
        public async Task SendMessage(WeatherReading reading)
        {
            var dtoReading = new WeatherReadingDto(reading);
            var jcontent = Newtonsoft.Json.JsonConvert.SerializeObject(dtoReading);
            await Clients.All.SendAsync("newForecast", jcontent);
        }
    }
}
