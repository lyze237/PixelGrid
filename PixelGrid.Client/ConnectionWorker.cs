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
        try
        {
            logger.LogInformation("Creating job with id {Id} and project {File} of type {Type} and {Files} files", id,
                projectFilename, type, filePaths.Length);

            var channel = GrpcChannel.ForAddress(options.Value.Url);
            var headers = new Metadata { { "Authorization", $"Bearer {options.Value.Token}" } };

            var filesChannel = new FilesControllerProto.FilesControllerProtoClient(channel);

            foreach (var requestPath in filePaths)
            {
                var path = Path.Combine(options.Value.Workdir, requestPath);

                logger.LogInformation("Checking file path {Path}", path);

                var dirPath = Path.GetDirectoryName(path);
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);

                var file = new FileInfo(path);

                if (!file.Exists)
                {
                    logger.LogInformation("File {Path} doesn't exist on client, downloading.", path);
                    await RequestFile(filesChannel, requestPath, path, headers);
                }
                else
                {
                    await RequestChunks(filesChannel, requestPath, path, headers);
                    return;
                }
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Something happened while downloading chunks for {Project}", projectFilename);
        }
    }

    private async Task RequestChunks(FilesControllerProto.FilesControllerProtoClient filesChannel, string requestPath,
        string path, Metadata headers)
    {
        var buffer = new byte[4096];
        int bytesRead;
        await using var currentFileStream = File.OpenRead(path);
        await using var newFileStream = File.Create(path + $".{Random.Shared.NextInt64()}.tmp");

        var request = filesChannel.CompareFileChunks(headers);

        await request.RequestStream.WriteAsync(new CompareFileChunksRequest
        {
            FileName = requestPath
        });

        while ((bytesRead = await currentFileStream.ReadAsync(buffer)) > 0)
        {
            var hash = SHA256.HashData(buffer.AsSpan(0, bytesRead));

            await request.RequestStream.WriteAsync(new CompareFileChunksRequest
            {
                Hash = ByteString.CopyFrom(hash)
            });

            await request.ResponseStream.MoveNext();
            var content = request.ResponseStream.Current;

            if (content.HasContent)
                await newFileStream.WriteAsync(content.Content.ToByteArray());
            else if (content.Status.HasOverride)
                await newFileStream.WriteAsync(buffer.AsMemory(0, bytesRead));
            else if (content.Status.HasEof)
                break;
        }

        await request.RequestStream.CompleteAsync();

        while (await request.ResponseStream.MoveNext())
        {
            var content = request.ResponseStream.Current;

            await newFileStream.WriteAsync(content.Content.ToByteArray());
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
            chunk.Content.WriteTo(fileStream);
        }
    }
}