using FluentResults.Extensions.AspNetCore;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelGrid.Server.Services;
using PixelGrid.Shared.Models.Controller;

namespace PixelGrid.Server.Controllers;

/// <summary>
/// Represents a controller for managing render jobs.
/// </summary>
[Authorize(Policy = "RenderClient")]
[Route("api/[controller]")]
[ApiController]
public class FilesController(FilesService filesService) : ControllerBase
{
    public Task<ActionResult> RequestProjectFileList(RequestProjectFileListRequest request,
        ServerCallContext context) =>
        filesService.RequestProjectFileList(request).ToActionResult();

    public async Task<ActionResult> RequestFileLength(RequestFileLengthRequest request,
        ServerCallContext context) =>
        filesService.GetFileLength(request).ToActionResult();

    public async Task<IActionResult> RequestFile(RequestFileRequest request)
    {
        var result = await filesService.RequestFile(request);
        return result.IsFailed
            ? result.ToResult().ToActionResult()
            : PhysicalFile(result.Value, "application/octet-stream");
    }

}