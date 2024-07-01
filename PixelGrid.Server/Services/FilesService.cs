using Google.Protobuf;
using System.Security.Cryptography;
using Grpc.Core;
using Microsoft.Extensions.Options;
using PixelGrid.Server.Database.Entities;
using PixelGrid.Server.Database.Repositories;
using PixelGrid.Server.Infra.Exceptions;
using PixelGrid.Server.Options;

namespace PixelGrid.Server.Services;

/// <summary>
/// Service to manage files and file operations.
/// </summary>
public class FilesService(
    ProjectRepository projectRepository,
    IOptions<RendererOptions> options,
    ILogger<FilesService> logger)
{
    public async Task<RequestProjectFileListResponse> RequestProjectFileList(RequestProjectFileListRequest request)
    {
        var project = await projectRepository.GetByIdAsync(request.ProjectId);
        if (project == null)
            throw new EntityNotFoundException<ProjectEntity>("Project not found");

        var projectDir = Path.Combine(options.Value.Workdir, project.Id.ToString());

        return new RequestProjectFileListResponse
        {
            Path =
            {
                Directory
                    .GetFiles(projectDir, "*", SearchOption.AllDirectories)
                    .Select(d => d[(projectDir.Length + 1)..]).ToArray()
            }
        };
    }

    /// <summary>
    /// Retrieves the length of a file.
    /// </summary>
    /// <param name="request">The request containing the file name.</param>
    /// <returns>The response containing the length of the file.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the file does not exist.</exception>
    public RequestFileLengthResponse GetFileLength(RequestFileLengthRequest request)
    {
        logger.LogInformation("Requesting file metadata for {File}", request.FileName);

        var path = Path.Combine(options.Value.Workdir, request.ProjectId.ToString(), request.FileName);
        if (!File.Exists(path))
            throw new FileNotFoundException();

        logger.LogInformation("Requesting path metadata for {File}", path);

        var file = new FileInfo(path);

        return new RequestFileLengthResponse
        {
            Length = file.Length
        };
    }

    /// <summary>
    /// Requests a file and streams its contents to the client.
    /// </summary>
    /// <param name="request">The request containing the file information.</param>
    /// <param name="responseStream">The server stream to write the file content to.</param>
    /// <exception cref="FileNotFoundException">Thrown if the file does not exist.</exception>
    public async Task RequestFile(RequestFileRequest request, IServerStreamWriter<RequestFileResponse> responseStream)
    {
        logger.LogInformation("Requesting file {File}", request.FileName);

        var path = Path.Combine(options.Value.Workdir, request.ProjectId.ToString(), request.FileName);
        if (!File.Exists(path))
            throw new FileNotFoundException();

        logger.LogInformation("Requesting path {File}", path);

        await using var file = File.OpenRead(path);
        var buffer = new byte[1024];
        int bytesRead;

        while ((bytesRead = file.Read(buffer, 0, buffer.Length)) > 0)
        {
            logger.LogInformation("Sending {Bytes} bytes", bytesRead);
            await responseStream.WriteAsync(new RequestFileResponse
            {
                Content = ByteString.CopyFrom(buffer, 0, bytesRead)
            });
        }
    }

    /// <summary>
    /// Compares the chunks of a file received from the client with the corresponding chunks on the server.
    /// </summary>
    /// <param name="requestStream">The stream of CompareFileChunksRequest objects representing the chunks of the file received from the client.</param>
    /// <param name="responseStream">The stream to send CompareFileChunksResponse objects back to the client.</param>
    /// <exception cref="EndOfStreamException">Thrown if the request stream has ended prematurely before receiving all the expected chunks.</exception>
    /// <exception cref="ArgumentException">Thrown if the first request does not contain the file name.</exception>
    public async Task CompareFileChunks(IAsyncStreamReader<CompareFileChunksRequest> requestStream,
        IServerStreamWriter<CompareFileChunksResponse> responseStream)
    {
        logger.LogInformation("Requesting file chunks");

        if (!await requestStream.MoveNext())
            throw new EndOfStreamException();

        var buffer = new byte[4096];
        int bytesRead;
        var fileRequest = requestStream.Current;
        if (fileRequest.HasHash)
            throw new ArgumentException("First request needs to contain file name");

        var fileStream = OpenFile(requestStream.Current.FileInfo.FileName, requestStream.Current.FileInfo.ProjectId);
        logger.LogInformation("Requesting file chunks from {File}", fileRequest.FileInfo.FileName);

        await foreach (var stream in requestStream.ReadAllAsync())
        {
            var clientHash = stream.Hash;

            logger.LogInformation("Received hash {Hash}", clientHash.Length);

            bytesRead = await fileStream.ReadAsync(buffer);
            if (bytesRead == 0)
            {
                logger.LogInformation("Our file is empty, sending EOF");
                await responseStream.WriteAsync(new CompareFileChunksResponse
                {
                    Status = new ChunkStatus
                    {
                        Eof = true
                    }
                });
                break;
            }

            var serverHash = SHA256.HashData(buffer.AsSpan(0, bytesRead));

            if (clientHash.SequenceEqual(serverHash))
            {
                await responseStream.WriteAsync(new CompareFileChunksResponse
                {
                    Status = new ChunkStatus
                    {
                        Override = true
                    }
                });
            }
            else
            {
                await responseStream.WriteAsync(new CompareFileChunksResponse
                {
                    Content = ByteString.CopyFrom(buffer, 0, bytesRead)
                });
            }
        }

        while ((bytesRead = await fileStream.ReadAsync(buffer)) > 0)
        {
            await responseStream.WriteAsync(new CompareFileChunksResponse
            {
                Content = ByteString.CopyFrom(buffer, 0, bytesRead)
            });
        }

        logger.LogInformation("Finished sending all chunks");
    }

    private FileStream OpenFile(string fileName, long projectId)
    {
        logger.LogInformation("Checking file path {FileName} in {Project} project", fileName, projectId);

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("Bad file name", nameof(fileName));

        var path = Path.Combine(options.Value.Workdir, projectId.ToString(), fileName);
        if (!File.Exists(path))
            throw new FileNotFoundException();

        logger.LogInformation("Reading file {Path}", path);

        return File.OpenRead(path);
    }
}