using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace BlazorTest.Shared
{
    public class GroceryHub : Hub
    {
        public const string HubUrl = "/categoryUpdates";

        public async Task SomethingChanged()
        {
            await Clients.All.SendAsync("SomethingChanged");
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"{Context.ConnectionId} connected");
            return base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception e)
        {
            Console.WriteLine($"Disconnected {e?.Message} {Context.ConnectionId}");
            await base.OnDisconnectedAsync(e);
        }
    }
}
