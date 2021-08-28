using BlazorTest.Data.BlazorTest;
using BlazorTestDB.Data.BlazorTest;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorTest.Data
{
    public class WeatherForecastService
    {
        private readonly GroceryContext _context;
        public WeatherForecastService(GroceryContext context)
        {
            _context = context;
        }
        public async Task<List<WeatherForecast>>
            GetForecastAsync(string strCurrentUser)
        {
            // Get Weather Forecasts  
            return await _context.WeatherForecast
                 // Only get entries for the current logged in user
                 .Where(x => x.UserName == strCurrentUser)
                 // Use AsNoTracking to disable EF change tracking
                 // Use ToListAsync to avoid blocking a thread
                 .AsNoTracking().ToListAsync();
        }

        public Task<WeatherForecast> CreateForecastAsync(WeatherForecast objWeatherForecast)
        {
            _context.WeatherForecast.Add(objWeatherForecast);
            _context.SaveChanges();
            return Task.FromResult(objWeatherForecast);
        }

        public Task<bool> UpdateForecastAsync(WeatherForecast objWeatherForecast)
        {
            var ExistingWeatherForecast =
                _context.WeatherForecast
                .Where(x => x.Id == objWeatherForecast.Id)
                .FirstOrDefault();
            if (ExistingWeatherForecast != null)
            {
                ExistingWeatherForecast.Date =
                    objWeatherForecast.Date;
                ExistingWeatherForecast.Summary =
                    objWeatherForecast.Summary;
                ExistingWeatherForecast.TemperatureC =
                    objWeatherForecast.TemperatureC;
                ExistingWeatherForecast.TemperatureF =
                    objWeatherForecast.TemperatureF;
                _context.SaveChanges();
            }
            else
            {
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }

        public Task<bool> DeleteForecastAsync(WeatherForecast objWeatherForecast)
        {
            var ExistingWeatherForecast =
                _context.WeatherForecast
                .Where(x => x.Id == objWeatherForecast.Id)
                .FirstOrDefault();
            if (ExistingWeatherForecast != null)
            {
                _context.WeatherForecast.Remove(ExistingWeatherForecast);
                _context.SaveChanges();
            }
            else
            {
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }
    }
}
