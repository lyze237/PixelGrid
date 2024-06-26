using System.Globalization;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using PixelGrid.Client.Extensions;
using PixelGrid.Client.Options;
using PixelGrid.Shared.Hubs;
using TypedSignalR.Client;

namespace PixelGrid.Client;

public class ConnectionWorker(IOptions<RendererOptions> rendererOptions, ILogger<ConnectionWorker> logger) : BackgroundService
{
    private async Task<HubConnection> CreateHubConnection(CancellationToken stoppingToken)
    {
        var connection = new HubConnectionBuilder()
            .WithUrl(rendererOptions.Value.Url.AppendIfNeeded("/") + "hubs/render",
                options => options.AccessTokenProvider = () => Task.FromResult(rendererOptions.Value.Token)!)
            .WithAutomaticReconnect()
            .Build();

        connection.Closed += error =>
        {
            logger.LogError(error, "Connection closed");
            return Task.CompletedTask;
        };
        connection.Reconnected += msg =>
        {
            logger.LogInformation("Reconnected {Msg}", msg);
            return Task.CompletedTask;
        };
        connection.Reconnecting += error =>
        {
            logger.LogError(error, "Reconnecting");
            return Task.CompletedTask;
        };

        connection.On<string, string>("ReceiveMessage",
            (user, message) => { logger.LogInformation("{User}: {Message}", user, message); });

        await connection.StartAsync(stoppingToken);
        logger.LogInformation("Connection state is {State}", connection.State);
        return connection;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var hub = await CreateHubConnection(stoppingToken);
        var proxy = hub.CreateHubProxy<IRenderHub.IServer>(stoppingToken);

        hub.Register<IRenderHub.IClient>(new HubReceiver());
        
        while (!stoppingToken.IsCancellationRequested)
        {
            await proxy.ClientToServer(DateTime.Now.ToString(CultureInfo.InvariantCulture));
            await Task.Delay(5000, stoppingToken);
        }
    }
}

public class HubReceiver : IRenderHub.IClient
{
    public async Task ServerToClient(string message)
    {
        Console.WriteLine($"Heeey~ {message}");
    }
}