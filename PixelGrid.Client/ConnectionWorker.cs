using System.Globalization;
using System.Net.Http.Json;
using System.Security.Cryptography;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using PixelGrid.Client.Extensions;
using PixelGrid.Client.Options;
using PixelGrid.Client.renderer;
using PixelGrid.Client.Services;
using PixelGrid.Shared.Hubs;
using PixelGrid.Shared.Models.Controller;
using TypedSignalR.Client;

namespace PixelGrid.Client;

public class ConnectionWorker(
    RenderFactory renderFactory,
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
                await proxy.RegisterProgram(type, version, renderFactory.GetRenderer(type, exe).GetCapabilities());
            }
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            await proxy.ClientToServer(DateTime.Now.ToString(CultureInfo.InvariantCulture));
            await Task.Delay(5000, stoppingToken);
        }
    }
}

public class ConnectionReceiver(
    IApiClient apiClient,
    IOptions<RendererOptions> options,
    ILogger<ConnectionReceiver> logger)
    : IRenderHub.IClient
{
    private readonly GrpcChannel grpc = GrpcChannel.ForAddress(options.Value.Url);
    private readonly Metadata headers = new() {{"Authorization", $"Bearer {options.Value.Token}"}};

    public async Task ServerToClient(string message)
    {
        logger.LogInformation("Heeey~ {Message}", message);
    }

    public async Task ForceDisconnect(string reason)
    {
        logger.LogInformation("Aw man we got disconnected: {Reason}", reason);
    }

    public async Task AssignProject(long projectId)
    {
        try
        {
            logger.LogInformation("Got project id {Project} assigned", projectId);

            var response = await apiClient.RequestProjectFileList(new RequestProjectFileListRequest(projectId));

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError(response.Error, "Response is errored");
                return;
            }

            var projectDirectory = Path.Combine(options.Value.Workdir, projectId.ToString());
            if (!Directory.Exists(projectDirectory))
                Directory.CreateDirectory(projectDirectory);

            foreach (var responseFile in response.Content.FilePath)
            {
                var responsePath = Path.Combine(projectDirectory, responseFile);

                logger.LogInformation("Checking file path {Path}", responsePath);

                var responseDirectory = Path.GetDirectoryName(responsePath);
                if (!Directory.Exists(responseDirectory))
                    Directory.CreateDirectory(responseDirectory);

                if (File.Exists(responsePath))
                    await RequestChunks(responseFile, responsePath, projectId);
                else
                    await RequestFile(responseFile, responsePath, projectId);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Couldn't start job for project {Project}", projectId);
        }
    }

    private async Task RequestChunks(string responseFile, string responsePath, long projectId)
    {
        logger.LogInformation("File exists, checking chunks for {File}", responseFile);

        var buffer = new byte[4096];
        int bytesRead;
        await using var currentFileStream = File.OpenRead(responsePath);
        await using var newFileStream = File.Create($"{responsePath}.{Random.Shared.NextInt64()}.tmp");

        var chunksChannel = new ChunksControllerProto.ChunksControllerProtoClient(grpc);
        var request = chunksChannel.CompareFileChunks(headers);

        await request.RequestStream.WriteAsync(new CompareFileChunksRequest
        {
            FileInfo = new CompareFileChunksFileInformation
            {
                FileName = responseFile,
                ProjectId = projectId
            }
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

    private async Task RequestFile(string responseFile, string responsePath, long projectId)
    {
        logger.LogInformation("File doesn't exists, downloading {File}", responseFile);

        var request = await apiClient.RequestFile(new RequestFileRequest(projectId, responseFile));
        await using var responseStream = await request.ReadAsStreamAsync();

        await using var fileStream = File.Create(responsePath);
        await responseStream.CopyToAsync(fileStream);
    }
}