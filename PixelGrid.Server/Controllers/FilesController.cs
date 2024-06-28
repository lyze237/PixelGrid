using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using PixelGrid.Server.Services;

namespace PixelGrid.Server.Controllers;

/// <summary>
/// Represents a controller for managing render jobs.
/// </summary>
[Authorize]
public class FilesController(
    FilesService filesService,
    RenderJobManagementService renderJobManagementService,
    ILogger<RenderJobController> logger) : FilesControllerProto.FilesControllerProtoBase
{
    /// <summary>
    /// Requests a files length.
    /// </summary>
    /// <param name="request">The request to get file metadata.</param>
    /// <param name="context">The server call context.</param>
    /// <returns>The response containing the file metadata.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the requested file is not found.</exception>
    public override async Task<RequestFileLengthResponse> RequestFileLength(RequestFileLengthRequest request,
        ServerCallContext context) =>
        filesService.GetFileLength(request);

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
        await filesService.RequestFile(request, responseStream);
    }

    /// <summary>
    /// Compares file chunks.
    /// </summary>
    /// <param name="requestStream">The stream of CompareFileChunksRequest.</param>
    /// <param name="responseStream">The stream of CompareFileChunksResponse.</param>
    /// <param name="context">The server call context.</param>
    /// <returns>The task representing the asynchronous operation.</returns>
    public override async Task CompareFileChunks(IAsyncStreamReader<CompareFileChunksRequest> requestStream,
        IServerStreamWriter<CompareFileChunksResponse> responseStream, ServerCallContext context) =>
        await filesService.CompareFileChunks(requestStream, responseStream);

}