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
    public partial class Stores
    {
        private string _hubUrl;
        private HubConnection _hubConnection;
        StoreService svc;
        RadzenDataGrid<Store> grid;

        // AuthenticationState is available as a CascadingParameter
        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }
        List<Store> stores;
        protected override async Task OnInitializedAsync()
        {
            // Get the current user
            svc = Service;

            await Refresh();

            var baseUrl = Env.IsDevelopment() ? navigationManager.BaseUri : "http://localhost";
            _hubUrl = baseUrl.TrimEnd('/') + StoreHub.HubUrl;

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_hubUrl)
                .Build();

            _hubConnection.On("SomethingChanged", Refresh);

            await _hubConnection.StartAsync();
        }

        public async Task Refresh()
        {
            stores = await svc.GetStoresAsync();
            await InvokeAsync(() => StateHasChanged());
        }

        void EditRow(Store store)
        {
            grid.EditRow(store);
        }

        async Task OnUpdateRow(Store store)
        {
            await @Service.UpdateStoresync(store);
            await grid.Reload();
            await _hubConnection.SendAsync("SomethingChanged");
        }

        async Task SaveRow(Store store)
        {
            await grid.UpdateRow(store);
            await grid.Reload();
            await _hubConnection.SendAsync("SomethingChanged");
        }

        void CancelEdit(Store store)
        {
            grid.CancelEditRow(store);
        }

        async Task DeleteRow(Store store)
        {
            if (stores.Contains(store))
            {
                await @Service.DeleteStoreAsync(store);
                await grid.Reload();
                await _hubConnection.SendAsync("SomethingChanged");
            }
            else
            {
                grid.CancelEditRow(store);
            }
        }

        void InsertRow()
        {
            grid.InsertRow(new Store());
        }

        async Task OnCreateRow(Store store)
        {
            await @Service.CreateStoreAsync(store);
            await grid.Reload();
            await _hubConnection.SendAsync("SomethingChanged");
        }
    }
}
