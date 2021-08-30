
using BlazorTest.Data;
using BlazorTest.Data.Data.Models;
using BlazorTest.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Radzen.Blazor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorTest.Pages
{
    public partial class Groceries
    {
        private string _hubUrl;
        private HubConnection _hubConnection;
        GroceryService svc;
        RadzenDataGrid<Grocery> grid;

        // AuthenticationState is available as a CascadingParameter
        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }
        List<Grocery> groceries;
        List<Category> categories;
        protected override async Task OnInitializedAsync()
        {
            // Get the current user
            svc = Service;

            await Refresh();

            var baseUrl = Env.IsDevelopment() ? navigationManager.BaseUri : "http://localhost";
            _hubUrl = baseUrl.TrimEnd('/') + GroceryHub.HubUrl;

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_hubUrl)
                .Build();

            _hubConnection.On("SomethingChanged", Refresh);

            await _hubConnection.StartAsync();
        }

        public async Task Refresh()
        {
            groceries = await svc.GetGroceriesAsync();
            categories = await svc.GetCategoriesAsync();
            await InvokeAsync(() => StateHasChanged());
        }

        void EditRow(Grocery grocery)
        {
            grid.EditRow(grocery);
        }

        async Task OnUpdateRow(Grocery grocery)
        {
            await @Service.UpdateGroceryAsync(grocery);
            await grid.Reload();
            await _hubConnection.SendAsync("SomethingChanged");
        }

        async Task SaveRow(Grocery grocery)
        {
            await grid.UpdateRow(grocery);
            await grid.Reload();
            await _hubConnection.SendAsync("SomethingChanged");
        }

        void CancelEdit(Grocery grocery)
        {
            grid.CancelEditRow(grocery);

            //// For production
            //var orderEntry = dbContext.Entry(order);
            //if (orderEntry.State == EntityState.Modified)
            //{
            //    orderEntry.CurrentValues.SetValues(orderEntry.OriginalValues);
            //    orderEntry.State = EntityState.Unchanged;
            //}
        }

        async Task DeleteRow(Grocery grocery)
        {
            if (groceries.Contains(grocery))
            {
                await @Service.DeleteGroceryAsync(grocery);
                await grid.Reload();
                await _hubConnection.SendAsync("SomethingChanged");
            }
            else
            {
                grid.CancelEditRow(grocery);
            }
        }

        void InsertRow()
        {
            grid.InsertRow(new Grocery());
        }

        async Task OnCreateRow(Grocery grocery)
        {
            await @Service.CreateGroceryAsync(grocery);
            await grid.Reload();
            await _hubConnection.SendAsync("SomethingChanged");
        }
    }
}
