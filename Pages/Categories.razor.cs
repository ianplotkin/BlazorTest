
using BlazorTest.Data;
using BlazorTest.Data.Data.Models;
using BlazorTest.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorTest.Pages
{
    public partial class Categories
    {
        private string _hubUrl;
        private HubConnection _hubConnection;
        CategoryService svc;

        // AuthenticationState is available as a CascadingParameter
        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }
        List<Category> categories;
        protected override async Task OnInitializedAsync()
        {
            // Get the current user
            svc = Service;
            // Get the forecasts for the current user
            // We access WeatherForecastService using @Service

            await Refresh();

            var baseUrl = Env.IsDevelopment() ? navigationManager.BaseUri : "http://localhost";
            _hubUrl = baseUrl.TrimEnd('/') + CategoryHub.HubUrl;

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_hubUrl)
                .Build();

            _hubConnection.On("SomethingChanged", Refresh);

            await _hubConnection.StartAsync();
        }

        public async Task Refresh()
        {
            //Debug.WriteLine("refreshing...");
            //await this.jsConsole.LogAsync("refreshing");
            //var user = (await authenticationStateTask).User;
            categories = await svc.GetCategoriesAsync();
            await InvokeAsync(() => StateHasChanged());
        }

        Category objCategory = new Category();

        bool ShowPopup = false;
        void ClosePopup()
        {
            // Close the Popup
            ShowPopup = false;
        }
        void AddNewCategory()
        {
            objCategory = new Category { Id = 0 };
            ShowPopup = true;
        }

        async Task SaveCategory()
        {
            ShowPopup = false;

            var user = (await authenticationStateTask).User;

            if (objCategory.Id == 0)
            {
                var objNewCategory = new Category
                {
                    Name = objCategory.Name
                    // add created by etc
                };

                /*
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
                */
                var result = @Service.CreateCategoryAsync(objNewCategory);
            }
            else
            {
                // This is an update
                var result = @Service.UpdateCategoryAsync(objCategory);
            }
            // Get the forecasts for the current user
            categories = await @Service.GetCategoriesAsync();
            await _hubConnection.SendAsync("SomethingChanged");
        }
        void EditCategory(Category category)
        {
            objCategory = category;
            ShowPopup = true;
        }
        async Task DeleteCategory()
        {
            ShowPopup = false;
            var user = (await authenticationStateTask).User;
            var result = @Service.DeleteCategoryAsync(objCategory);
            categories = await @Service.GetCategoriesAsync();
            await _hubConnection.SendAsync("SomethingChanged");
        }
    }
}
