
using BlazorTest.Data;
using BlazorTest.Data.Models;
using BlazorTest.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Radzen.Blazor;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorTest.Pages
{
    public partial class Groceries
    {
        private HubConnection hubConnection;
        private GroceryService svc;
        private Grocery editingGrocery;
        private List<Grocery> groceries;
        private readonly List<Grocery> selectedGroceries = new();
        private List<Grocery> displayedGroceries;
        private List<Category> categories;
        private string addItemText;

        protected override async Task OnInitializedAsync()
        {
            svc = Service;

            var hubUrl = (Env.IsDevelopment() ? navigationManager.BaseUri.TrimEnd('/') : "http://localhost") + GroceryHub.HubUrl;
            hubConnection = new HubConnectionBuilder().WithUrl(hubUrl).Build();
            hubConnection.On("SomethingChanged", Refresh);
            hubConnection.On("SomethingChangedOnDialog", Refresh2);

            await hubConnection.StartAsync();
            await Refresh();
        }

        public async Task Refresh()
        {
            await jsConsole.LogAsync("REFRESHING...");
            groceries = await svc.GetGroceriesAsync();
            categories = await svc.GetCategoriesAsync();
            UpdateDisplayedGroceries();
            await InvokeAsync(() => StateHasChanged());
            //await hubConnection.SendAsync("SomethingChanged");
        }

        public async Task Refresh2()
        {
            System.Diagnostics.Debug.WriteLine("Refresh2");
        }

        public void UpdateDisplayedGroceries()
        {
            displayedGroceries = string.IsNullOrEmpty(addItemText) ? groceries :
                groceries.Where(g => g.Name.ToUpper().Contains(addItemText.ToUpper())).ToList();
        }

        void AddTextChanged(string value)
        {
            addItemText = value;
            UpdateDisplayedGroceries();
        }

        void AddNewGrocery()
        {
            editingGrocery = new Grocery { Id = 0 };
            //showPopup = true;
        }

        async Task Submit(Grocery grocery)
        {
            if (grocery.Id == 0)
            {
                await @Service.CreateGroceryAsync(grocery);
            }
            else
            {
                await @Service.UpdateGroceryAsync(grocery);
            }

            await Refresh();
            await hubConnection.SendAsync("SomethingChanged");
        }

       
        async Task IncrementAmount()
        {
            editingGrocery.DefaultAmount += 1;
            await InvokeAsync(() => StateHasChanged());
            await hubConnection.SendAsync("SomethingChangedOnDialog");
            System.Diagnostics.Debug.WriteLine("++++");
        }

        async Task DecrementAmount()
        {
            editingGrocery.DefaultAmount -= 1;
            await InvokeAsync(() => StateHasChanged());
            await hubConnection.SendAsync("SomethingChangedOnDialog");
            System.Diagnostics.Debug.WriteLine("----");
        }

        void OnChange(bool isChecked, Grocery grocery)
        {
            if (isChecked)
            {
                selectedGroceries.Add(grocery);
                Debug.WriteLine($"Added grocery {grocery.Name}");
            }
            else
            {
                selectedGroceries.Remove(grocery);
                Debug.WriteLine($"Removed grocery {grocery.Name}");
            }

            Debug.WriteLine("Selected groceries: " + string.Join(", ", selectedGroceries.Select(g => g.Name)));
        }
    }
}
