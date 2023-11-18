using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using PixelGrid.Client.Commands;
using PixelGrid.Client.Options;
using PixelGrid.Client.Policies;
using PixelGrid.Shared.Hubs;
using TypedSignalR.Client;

namespace PixelGrid.Client;

public class ConnectionWorker(IOptions<ConnectionOptions> connectionOptions, ITestHub.ITestHubClient testHubClient, ILogger<TestHubReceiver> logger) : BackgroundService
{
    private readonly ConnectionOptions connectionOptions = connectionOptions.Value;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var connection = new HubConnectionBuilder()
            .WithUrl(connectionOptions.Url, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(connectionOptions.Token)!;
            })
            .WithAutomaticReconnect(new ReconnectRetryPolicy())
            .Build();
        
        SetupLogging(connection);

        var hub = connection.CreateHubProxy<ITestHub.ITestHubServer>(cancellationToken: stoppingToken);
        using var receiver = connection.Register(testHubClient);
        
        await connection.StartAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await hub.SendMessage("Hewwo 2", "Woorld~");
                await connection.SendAsync("SendMessage", "Hewwo", "World", cancellationToken: stoppingToken);
            }

            await Task.Delay(1000, stoppingToken);
        }
    }

    private void SetupLogging(HubConnection connection)
    {
        connection.Closed += error =>
        {
            logger.LogError(error, "Lost connection");
            return Task.CompletedTask;
        };

        connection.Reconnecting += error =>
        {
            logger.LogInformation(error, "Reconnecting...");
            return Task.CompletedTask;
        };

        connection.Reconnected += id =>
        {
            logger.LogInformation("Reconnected with Id {id}", id);
            return Task.CompletedTask;
        };
    }
}