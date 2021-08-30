
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
    public partial class Categories
    {
        private string _hubUrl;
        private HubConnection _hubConnection;
        CategoryService svc;
        RadzenDataGrid<Category> grid;

        // AuthenticationState is available as a CascadingParameter
        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }
        List<Category> categories;
        protected override async Task OnInitializedAsync()
        {
            // Get the current user
            svc = Service;

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
            categories = await svc.GetCategoriesAsync();
            await InvokeAsync(() => StateHasChanged());
        }

        void EditRow(Category category)
        {
            grid.EditRow(category);
        }

        async Task OnUpdateRow(Category category)
        {
            await @Service.UpdateCategoryAsync(category);
            await grid.Reload();
            await _hubConnection.SendAsync("SomethingChanged");
        }

        async Task SaveRow(Category category)
        {
            await grid.UpdateRow(category);
            await grid.Reload();
            await _hubConnection.SendAsync("SomethingChanged");
        }

        void CancelEdit(Category category)
        {
            grid.CancelEditRow(category);

            //// For production
            //var orderEntry = dbContext.Entry(order);
            //if (orderEntry.State == EntityState.Modified)
            //{
            //    orderEntry.CurrentValues.SetValues(orderEntry.OriginalValues);
            //    orderEntry.State = EntityState.Unchanged;
            //}
        }

        async Task DeleteRow(Category category)
        {
            if (categories.Contains(category))
            {
                await @Service.DeleteCategoryAsync(category);
                await grid.Reload();
                await _hubConnection.SendAsync("SomethingChanged");
            }
            else
            {
                grid.CancelEditRow(category);
            }
        }

        void InsertRow()
        {
            grid.InsertRow(new Category());
        }

        async Task OnCreateRow(Category category)
        {
            await @Service.CreateCategoryAsync(category);
            await grid.Reload();
            await _hubConnection.SendAsync("SomethingChanged");
        }
    }
}
