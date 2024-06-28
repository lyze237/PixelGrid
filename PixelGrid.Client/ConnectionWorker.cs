using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using PixelGrid.Client.Extensions;
using PixelGrid.Client.Options;
using PixelGrid.Client.renderer;
using PixelGrid.Shared.Hubs;
using PixelGrid.Shared.Renderer;
using TypedSignalR.Client;

namespace PixelGrid.Client;

public class ConnectionWorker(
    RenderFactory factory,
    IServiceProvider serviceProvider,
    IOptions<RendererOptions> rendererOptions,
    ILogger<ConnectionWorker> logger) : BackgroundService
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

        hub.Register<IRenderHub.IClient>(serviceProvider.GetRequiredService<ConnectionReceiver>());

        foreach (var (type, versions) in rendererOptions.Value.Versions)
        {
            foreach (var (version, exe) in versions)
            {
                await proxy.RegisterProgram(type, version, factory.GetRenderer(type, exe).GetCapabilities());
            }
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            await proxy.ClientToServer(DateTime.Now.ToString(CultureInfo.InvariantCulture));
            await Task.Delay(5000, stoppingToken);
        }
    }
}

public class ConnectionReceiver(IOptions<RendererOptions> options, ILogger<ConnectionReceiver> logger)
    : IRenderHub.IClient
{
    public async Task ServerToClient(string message)
    {
        logger.LogInformation("Heeey~ {Message}", message);
    }

    public async Task ForceDisconnect(string reason)
    {
        logger.LogInformation("Aw man we got disconnected: {Reason}", reason);
    }

    public async Task CreateJob(long id, string[] filePaths, string projectFilename, RenderType type)
    {
        logger.LogInformation("Creating job with id {Id} and project {File} of type {Type} and {Files} files", id,
            projectFilename, type, filePaths.Length);

        var channel = GrpcChannel.ForAddress(options.Value.Url);
        var headers = new Metadata {{"Authorization", $"Bearer {options.Value.Token}"}};

        var renderJobChannel = new RenderJobControllerProto.RenderJobControllerProtoClient(channel);
        var filesChannel = new FilesControllerProto.FilesControllerProtoClient(channel);

        var jobsFolder = "C:\\Users\\lyze\\Desktop\\blenderTest\\jobs";

        foreach (var requestPath in filePaths)
        {
            var path = Path.Combine(jobsFolder, requestPath);

            logger.LogInformation("Checking file path {Path}", path);

            var dirPath = Path.GetDirectoryName(path);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            var file = new FileInfo(path);

            var metadataResponse = filesChannel.RequestFileMetadata(new RequestFileMetadataRequest
            {
                FileName = requestPath,
                Size = file.Length
            }, headers);

            if (metadataResponse.Size != file.Length)
            {
                logger.LogInformation("File sizes are different: {FileLength} != {ResponseLength}", file.Length,
                    metadataResponse.Size);

                await RequestFile(filesChannel, requestPath, path, headers);
            }
            else
            {
                using var sha256 = SHA256.Create();
                await using var stream = file.OpenRead();
                var computeHashAsync = await sha256.ComputeHashAsync(stream);

                if (!metadataResponse.Hash.ToByteArray().SequenceEqual(computeHashAsync))
                {
                    logger.LogInformation("Hashes are different, requesting new file for {Path}", path);
                    await RequestFile(filesChannel, requestPath, path, headers);
                }
            }
        }
    }

    private async Task RequestFile(FilesControllerProto.FilesControllerProtoClient filesChannel,
        string requestPath, string path,
        Metadata headers)
    {
        using var response = filesChannel.RequestFile(new RequestFileRequest
        {
            FileName = requestPath
        }, headers);

        await using var fileStream = File.Create(path);
        await foreach (var chunk in response.ResponseStream.ReadAllAsync())
        {
            logger.LogInformation("Receiving chunk {Chunk}", chunk.Content.Length);
            await fileStream.WriteAsync(chunk.Content.ToByteArray());
        }
    }
}