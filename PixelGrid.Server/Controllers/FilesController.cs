using FluentResults.Extensions.AspNetCore;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelGrid.Server.Services;
using PixelGrid.Shared.Models.Controller;

namespace PixelGrid.Server.Controllers;

[Authorize(Policy = "RenderClient")]
[Route("Api/[controller]")]
[ApiController]
public class FilesController(FilesService filesService) : ControllerBase
{
    [HttpGet("RequestProjectFileList")]
    public Task<ActionResult> RequestProjectFileList([FromQuery] RequestProjectFileListRequest request) =>
        filesService.RequestProjectFileList(request).ToActionResult();

    [HttpGet("RequestFileLength")]
    public async Task<ActionResult> RequestFileLength([FromQuery] RequestFileLengthRequest request) =>
        filesService.GetFileLength(request).ToActionResult();

    [HttpGet("RequestFile")]
    public async Task<IActionResult> RequestFile([FromQuery] RequestFileRequest request)
    {
        var result = await filesService.RequestFile(request);
        return result.IsFailed
            ? result.ToResult().ToActionResult()
            : PhysicalFile(result.Value, "application/octet-stream");
    }

}