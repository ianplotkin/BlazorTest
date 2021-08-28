
using BlazorTest.Data.BlazorTest;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

namespace BlazorTest.Pages
{
    public partial class FetchData
    {
        // AuthenticationState is available as a CascadingParameter
        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }
        List<WeatherForecast> forecasts;
        protected override async Task OnInitializedAsync()
        {
            // Get the current user
            var user = (await authenticationStateTask).User;
            // Get the forecasts for the current user
            // We access WeatherForecastService using @Service
            forecasts = await @Service.GetForecastAsync(user.Identity.Name);
        }
        WeatherForecast objWeatherForecast = new WeatherForecast();

        private Timer timer;

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                timer = new Timer();
                timer.Interval = 1000;
                timer.Elapsed += OnTimerInterval;
                timer.AutoReset = true;
                // Start the timer
                timer.Enabled = true;
            }
            base.OnAfterRender(firstRender);
        }

        private void OnTimerInterval(object sender, ElapsedEventArgs e)
        {
            InvokeAsync(() => StateHasChanged());
        }



        bool ShowPopup = false;
        void ClosePopup()
        {
            // Close the Popup
            ShowPopup = false;
        }
        void AddNewForecast()
        {
            // Make new forecast
            objWeatherForecast = new WeatherForecast();
            // Set Id to 0 so we know it is a new record
            objWeatherForecast.Id = 0;
            // Open the Popup
            ShowPopup = true;
        }
        async Task SaveForecast()
        {
            // Close the Popup
            ShowPopup = false;
            // Get the current user
            var user = (await authenticationStateTask).User;
            // A new forecast will have the Id set to 0
            if (objWeatherForecast.Id == 0)
            {
                // Create new forecast
                WeatherForecast objNewWeatherForecast = new WeatherForecast();
                objNewWeatherForecast.Date = System.DateTime.Now;
                objNewWeatherForecast.Summary = objWeatherForecast.Summary;
                objNewWeatherForecast.TemperatureC =
                Convert.ToInt32(objWeatherForecast.TemperatureC);
                objNewWeatherForecast.TemperatureF =
                Convert.ToInt32(objWeatherForecast.TemperatureF);
                objNewWeatherForecast.UserName = user.Identity.Name;
                // Save the result
                var result =
                @Service.CreateForecastAsync(objNewWeatherForecast);
            }
            else
            {
                // This is an update
                var result =
                @Service.UpdateForecastAsync(objWeatherForecast);
            }
            // Get the forecasts for the current user
            forecasts =
            await @Service.GetForecastAsync(user.Identity.Name);
        }
        void EditForecast(WeatherForecast weatherForecast)
        {
            // Set the selected forecast
            // as the current forecast
            objWeatherForecast = weatherForecast;
            // Open the Popup
            ShowPopup = true;
        }
        async Task DeleteForecast()
        {
            // Close the Popup
            ShowPopup = false;
            // Get the current user
            var user = (await authenticationStateTask).User;
            // Delete the forecast
            var result = @Service.DeleteForecastAsync(objWeatherForecast);
            // Get the forecasts for the current user
            forecasts =
            await @Service.GetForecastAsync(user.Identity.Name);
        }
    }
}
