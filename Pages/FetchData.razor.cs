
using BlazorTest.Data;
using BlazorTest.Data.BlazorTest;
using BlazorTest.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;

namespace BlazorTest.Pages
{
    public partial class FetchData
    {
        private string _hubUrl;
        private HubConnection _hubConnection;
        WeatherForecastService svc;

        // AuthenticationState is available as a CascadingParameter
        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }
        List<WeatherForecast> forecasts;
        protected override async Task OnInitializedAsync()
        {
            // Get the current user
            svc = Service;
            // Get the forecasts for the current user
            // We access WeatherForecastService using @Service

            await Refresh();

            string baseUrl = navigationManager.BaseUri;
            Debug.WriteLine("BASE URL: " + baseUrl);
            baseUrl = "http://localhost";
            _hubUrl = baseUrl.TrimEnd('/') + UpdateHub.HubUrl;

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_hubUrl)
                .Build();

            _hubConnection.On("SomethingChanged", Refresh);

            await _hubConnection.StartAsync();

            //await SendAsync($"[Notice] {_username} joined chat room.");
        }

        public async Task Refresh()
        {
            Debug.WriteLine("refreshing...");
            await this.jsConsole.LogAsync("refreshing");

            var user = (await authenticationStateTask).User;

            forecasts = await svc.GetForecastAsync(user.Identity.Name);
            await InvokeAsync(() => StateHasChanged());

        }

        WeatherForecast objWeatherForecast = new WeatherForecast();

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
            forecasts = await @Service.GetForecastAsync(user.Identity.Name);
            await _hubConnection.SendAsync("SomethingChanged");
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
            await _hubConnection.SendAsync("SomethingChanged");
        }
    }
}
