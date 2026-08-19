
using Microsoft.AspNetCore.SignalR;

namespace HotelManagementSystem.Hubs
{
    public class OrderHub : Hub
    {
        public async Task JoinKitchenGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Kitchen");
        }

        // 2. Waiter calls this when submitting an order
        //public async Task PlaceOrder(string tableName, string items)
        //{
        //    // Sends the order instantly to everyone in the "Kitchen" group
        //    await Clients.Group("Kitchen").SendAsync("NewOrderAlert", tableName, items);
        //}

        // 3. Chef calls this when food is ready
        //public async Task FoodIsReady(string tableName, string dishName)
        //{
        //    // Sends a notification to ALL connected staff
        //    await Clients.All.SendAsync("OrderReadyAlert", tableName, dishName);
        //}
    }
}
