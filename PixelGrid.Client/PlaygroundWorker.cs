using Grpc.Net.Client;
using Microsoft.AspNetCore.SignalR.Client;

namespace PixelGrid.Client;

public class PlaygroundWorker(ILogger<PlaygroundWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Register();
        var result = await Login();
        if (!result.Success)
            logger.LogInformation("Couldn't login");
        
        logger.LogInformation(result.Token);
        
        var connection = await CreateHubConnection(result.Token, stoppingToken);

        await connection.SendAsync("SendMessage", "Lyze", "Test", cancellationToken: stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
            await Task.Delay(1000, stoppingToken);

        await connection.StopAsync(stoppingToken);
    }

    private async Task<HubConnection> CreateHubConnection(string token, CancellationToken stoppingToken)
    {
        var connection = new HubConnectionBuilder().WithUrl("https://localhost:7232/hubs/chat",
                options => options.AccessTokenProvider = () => Task.FromResult(token)!)
            .WithAutomaticReconnect()
            .Build();

        connection.Closed += async error => logger.LogError(error, "Connection closed");
        connection.Reconnected += async msg => logger.LogInformation("Reconnected {msg}", msg);
        connection.Reconnecting += async error => logger.LogError(error, "Reconnecting");

        connection.On<string, string>("ReceiveMessage",
            (user, message) => { logger.LogInformation("{user}: {message}", user, message); });

        await connection.StartAsync(stoppingToken);
        logger.LogInformation("Connection state is {state}", connection.State);
        return connection;
    }

    private static async Task Register()
    {
        var channel = GrpcChannel.ForAddress("https://localhost:7232/");
        var client = new Auth.AuthClient(channel);
        await client.RegisterAsync(new AuthRegisterRequest
            {UserName = "test", Email = "test@example.com", Password = "Password123!"});
    }

    private async Task<AuthLoginResponse> Login()
    {
        var channel = GrpcChannel.ForAddress("https://localhost:7232/");
        var client = new Auth.AuthClient(channel);
        return await client.LoginAsync(new AuthLoginRequest {Email = "test@example.com", Password = "Password123!"});
    }
}