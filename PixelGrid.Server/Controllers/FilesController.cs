using System.Security.Cryptography;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using PixelGrid.Server.Services;

namespace PixelGrid.Server.Controllers;

/// <summary>
/// Represents a controller for managing render jobs.
/// </summary>
[Authorize]
public class FilesController(RenderJobManagementService renderJobManagementService, ILogger<RenderJobController> logger) : FilesControllerProto.FilesControllerProtoBase
{
    private static string dir = "C:\\Users\\lyze\\Desktop\\blenderTest\\Ansichten";

    /// <summary>
    /// Requests a files length and hash.
    /// </summary>
    /// <param name="request">The request to get file metadata.</param>
    /// <param name="context">The server call context.</param>
    /// <returns>The response containing the file metadata.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the requested file is not found.</exception>
    public override async Task<RequestFileMetadataResponse> RequestFileMetadata(RequestFileMetadataRequest request,
        ServerCallContext context)
    {
        logger.LogInformation("Requesting file metadata for {File}", request.FileName);
        
        var path = Path.Combine(dir, request.FileName);
        if (!File.Exists(path))
            throw new FileNotFoundException();
        
        logger.LogInformation("Requesting path metadata for {File}", path);

        var file = new FileInfo(path);

        if (request.Size == file.Length)
        {
            logger.LogInformation("File size {FileSize} is same as client {ClientSize} size, generating hash", file.Length, request.Size);
            
            using var sha256 = SHA256.Create();
            await using var stream = file.OpenRead();
            var computeHashAsync = await sha256.ComputeHashAsync(stream);

            return new RequestFileMetadataResponse
            {
                Size = file.Length,
                Hash = ByteString.CopyFrom(computeHashAsync)
            };
        }

        logger.LogInformation("File size {FileSize} is not same as client {ClientSize} size", file.Length, request.Size);
        
        return new RequestFileMetadataResponse
        {
            Size = file.Length
        };
    }

    /// <summary>
    /// Requests a file and sends it through a server stream.
    /// </summary>
    /// <param name="request">The request object containing the file name.</param>
    /// <param name="responseStream">The server stream to send the file content.</param>
    /// <param name="context">The server call context.</param>
    /// <exception cref="FileNotFoundException">Thrown if the requested file does not exist.</exception>
    [Authorize(Policy = "RenderClient")]
    public override async Task RequestFile(RequestFileRequest request,
        IServerStreamWriter<RequestFileResponse> responseStream, ServerCallContext context)
    {
        logger.LogInformation("Requesting file {File}", request.FileName);
        
        var path = Path.Combine(dir, request.FileName);
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
}