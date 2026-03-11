using Microsoft.AspNetCore.SignalR;

namespace TrustChain.Hubs;

public class VoteHub : Hub
{
    // Server pushes TO clients  no client methods needed
    // Frontend listens for: "ReceiveResults"
    // Frontend connects to: /hubs/vote

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"[SignalR] Client connected: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"[SignalR] Client disconnected: {Context.ConnectionId}");
        await base.OnDisconnectedAsync(exception);
    }
}