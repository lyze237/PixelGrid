using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using PixelGrid.Server.Services;

namespace PixelGrid.Server.Controllers;

[Authorize(Policy = "RenderClient")]
public class ChunksController(FilesService filesService) : ChunksControllerProto.ChunksControllerProtoBase
{
    public override async Task CompareFileChunks(IAsyncStreamReader<CompareFileChunksRequest> requestStream, IServerStreamWriter<CompareFileChunksResponse> responseStream, ServerCallContext context) => 
        await filesService.CompareFileChunks(requestStream, responseStream);
}