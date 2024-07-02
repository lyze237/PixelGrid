using Google.Protobuf;
using System.Security.Cryptography;
using FluentResults;
using Grpc.Core;
using Microsoft.Extensions.Options;
using OracleInternal.Sharding;
using PixelGrid.Server.Database.Entities;
using PixelGrid.Server.Database.Repositories;
using PixelGrid.Server.Infra.Exceptions;
using PixelGrid.Server.Options;
using PixelGrid.Shared.Models.Controller;

namespace PixelGrid.Server.Services;

public class FilesService(
    ProjectRepository projectRepository,
    IOptions<RendererOptions> options,
    ILogger<FilesService> logger)
{
    public async Task<Result<RequestProjectFileListResponse>> RequestProjectFileList(
        RequestProjectFileListRequest request)
    {
        var project = await projectRepository.GetByIdAsync(request.ProjectId);
        if (project == null)
            return Result.Fail("Project not found");

        var projectDir = Path.Combine(options.Value.Workdir, project.Id.ToString());

        var filePaths = Directory
            .GetFiles(projectDir, "*", SearchOption.AllDirectories)
            .Select(d => d[(projectDir.Length + 1)..]).ToArray();
        return Result.Ok(new RequestProjectFileListResponse(filePaths));
    }

    public Result<RequestFileLengthResponse> GetFileLength(RequestFileLengthRequest request)
    {
        logger.LogInformation("Requesting file metadata for {File} in {Project} project", request.FilePath, request.ProjectId);

        var path = Path.Combine(options.Value.Workdir, request.ProjectId.ToString(), request.FilePath);
        if (!File.Exists(path))
            return Result.Fail("Invalid file");

        logger.LogInformation("Requesting path metadata for {File}", path);

        var file = new FileInfo(path);
        return Result.Ok(new RequestFileLengthResponse(file.Length));
    }

    public async Task<Result<string>> RequestFile(RequestFileRequest request)
    {
        logger.LogInformation("Requesting file {File}", request.FilePath);

        var path = GetFilePath(request.ProjectId, request.FilePath);
        if (!File.Exists(path))
            return Result.Fail("Invalid file");

        logger.LogInformation("Requesting path {File}", path);
        return Result.Ok(path);
    }

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

    private FileStream OpenFile(string filePath, long projectId)
    {
        logger.LogInformation("Checking file path {FileName} in {Project} project", filePath, projectId);

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Bad file name", nameof(filePath));

        var path = GetFilePath(projectId, filePath);
        if (!File.Exists(path))
            throw new FileNotFoundException();

        logger.LogInformation("Reading file {Path}", path);

        return File.OpenRead(path);
    }

    private string GetFilePath(long projectId, string filePath) =>
        Path.Combine(options.Value.Workdir, projectId.ToString(), filePath);
}