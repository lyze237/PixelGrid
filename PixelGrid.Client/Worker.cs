using Microsoft.AspNetCore.SignalR.Client;

namespace PixelGrid.Client;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> logger;
    private readonly HubConnection connection;

    public Worker(ILogger<Worker> logger)
    {
        this.logger = logger;
        connection = new HubConnectionBuilder().WithUrl("http://localhost:5249/hubs/chat", options =>
        {
            options.AccessTokenProvider = () => Task.FromResult("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYmYiOjE3MTg4NzY5MzEsImV4cCI6MTcxODg4MDUzMSwiaWF0IjoxNzE4ODc2OTMxLCJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjUyNDkiLCJhdWQiOiJodHRwOi8vbG9jYWxob3N0OjUyNDkifQ.KfOa2cJI70o2dCReKxk2YHCdfSCKYmju6bua-IcyXWQ")!;
        })
        .WithAutomaticReconnect()
        .Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        connection.Closed += async error => logger.LogError(error, "Connection closed");
        connection.Reconnected += async msg => logger.LogInformation("Reconnected {msg}", msg);
        connection.Reconnecting += async error => logger.LogError(error, "Reconnecting");

        connection.On<string, string>("ReceiveMessage", (user, message) => { logger.LogInformation("{user}: {message}", user, message); });
        
        await connection.StartAsync(stoppingToken);
        logger.LogInformation("Connection state is {state}", connection.State);

        await connection.SendAsync("SendMessage", "Lyze", "Test", cancellationToken: stoppingToken);
        
        while (!stoppingToken.IsCancellationRequested)
            await Task.Delay(1000, stoppingToken);

        await connection.StopAsync(stoppingToken);
    }
}