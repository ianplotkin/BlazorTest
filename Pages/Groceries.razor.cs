
using BlazorTest.Data;
using BlazorTest.Data.Data.Models;
using BlazorTest.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Radzen.Blazor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorTest.Pages
{
    public partial class Groceries
    {
        private HubConnection hubConnection;
        private GroceryService svc;
        private RadzenDataGrid<Grocery> grid;
        private Grocery editingGrocery;
        private List<Grocery> groceries;
        private List<Grocery> displayedGroceries;
        private List<Category> categories;
        private bool showPopup = false;
        private string addItemText;

        // AuthenticationState is available as a CascadingParameter
        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }

        protected override async Task OnInitializedAsync()
        {
            svc = Service;



            var hubUrl = (Env.IsDevelopment() ? navigationManager.BaseUri.TrimEnd('/') : "http://localhost") + GroceryHub.HubUrl;
            hubConnection = new HubConnectionBuilder().WithUrl(hubUrl).Build();
            hubConnection.On("SomethingChanged", Refresh);
            await hubConnection.StartAsync();

            await Refresh();
        }

        public async Task Refresh()
        {
            //await jsConsole.LogAsync("REFRESHING...");
            groceries = await svc.GetGroceriesAsync();
            categories = await svc.GetCategoriesAsync();
            UpdateDisplayedGroceries();
            await InvokeAsync(() => StateHasChanged());
            await hubConnection.SendAsync("SomethingChanged");
        }

        public void UpdateDisplayedGroceries()
        {
            displayedGroceries = string.IsNullOrEmpty(addItemText) ? groceries :
                groceries.Where(g => g.Name.ToUpper().Contains(addItemText.ToUpper())).ToList();
        }

        void ClosePopup()
        {
            showPopup = false;
        }

        void EditGrocery(Grocery grocery)
        {
            editingGrocery = grocery.Clone();
            showPopup = true;
        }

        async Task SaveGrocery()
        {
            if (editingGrocery.Id == 0)
            {
                await @Service.CreateGroceryAsync(editingGrocery);
            }
            else
            {
                await @Service.UpdateGroceryAsync(editingGrocery);
            }

            showPopup = false;
            await Refresh();
        }

        async Task DeleteGrocery()
        {
            // Close the Popup
            showPopup = false;
            await Service.DeleteGroceryAsync(editingGrocery);
            await Refresh();
        }

        void AddTextChanged(string value)
        {
            addItemText = value;
            UpdateDisplayedGroceries();
        }

        void AddNewGrocery()
        {
            editingGrocery = new Grocery { Id = 0 };
            showPopup = true;
        }
    }
}
