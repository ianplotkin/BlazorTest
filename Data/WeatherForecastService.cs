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
    }
}
