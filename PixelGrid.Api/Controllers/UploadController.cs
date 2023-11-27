using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PixelGrid.Api.Data;
using PixelGrid.Api.Managers;
using PixelGrid.Api.Options;
using File = PixelGrid.Api.Data.File;

namespace PixelGrid.Api.Controllers;


[Authorize]
public class UploadController(ApplicationDbContext dbContext, UserManager<User> userManager, IOptions<FolderOptions> folderOptions, ChunkManager chunkManager, ILogger<UploadController> logger) : Controller
{
    private readonly FolderOptions folderOptions = folderOptions.Value;

    [HttpGet]
    public async Task<IActionResult> Index(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("No id given.");

        var user = await userManager.GetUserAsync(User) ?? throw new ArgumentException("User is null?");
        var project = await dbContext.Projects
            .Include(c => c.Files)
            .Include(c => c.SharedWith)
            .FirstOrDefaultAsync(c => c.Id == id && c.Owner == user);

        if (project == null)
            return BadRequest("Id not found or not owner.");

        return View(project);
    }
    
    [RequestSizeLimit(10000000)]
    [HttpPost]
    public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] string? dzfullpath, [FromForm] string dzuuid, [FromForm] ulong dzchunkindex, [FromForm] ulong dztotalfilesize, [FromForm] ulong dzchunksize, [FromForm] ulong dztotalchunkcount, [FromForm] ulong dzchunkbyteoffset, [FromForm] string projectId)
    {
        if (string.IsNullOrWhiteSpace(projectId))
            return BadRequest("No id given.");

        var user = await userManager.GetUserAsync(User) ?? throw new ArgumentException("User is null?");
        var project = await dbContext.Projects
            .Include(c => c.SharedWith)
            .FirstOrDefaultAsync(c => c.Id == projectId && c.Owner == user);
        
        if (project == null)
            return BadRequest("Id not found or not owner.");

        var chunk = new Chunk(dzuuid, dzchunkindex, dztotalfilesize, dzchunksize, dztotalchunkcount, dzchunkbyteoffset, dzfullpath, file.FileName);
        logger.LogInformation("Saving chunk: {chunk}", chunk);

        if (!chunk.ValidFileName())
            return BadRequest("Invalid filename");

        if (!chunk.ValidFilePath())
            return BadRequest("Invalid filepath");
        
        await chunkManager.StoreChunk(chunk, file);

        if (!chunkManager.IsValid(chunk.Uuid))
            return BadRequest("Chunk upload received garbled data.");
        
        if (chunkManager.IsComplete(chunk.Uuid))
        {
            var fileSize = await chunkManager.Save(project, chunk.Uuid);

            var dbFile = await dbContext.Files.FirstOrDefaultAsync(f => f.Path == dzfullpath);
            if (dbFile == null)
                dbContext.Files.Add(new File(project.Id, chunk.FullPath ?? chunk.FileName, fileSize));
            else
                dbFile.UpdateFileSize(fileSize);
            await dbContext.SaveChangesAsync();
                
            return Ok("Completed!~");
        }

        return Ok();
    }

    public async Task<IActionResult> Delete(string id, string fileName)
    {
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(fileName))
            return BadRequest("No id given.");

        var user = await userManager.GetUserAsync(User) ?? throw new ArgumentException("User is null?");

        var dbFile = await dbContext.Files
            .Include(file => file.Project)
            .FirstOrDefaultAsync(f => f.Path == fileName && f.Project.Owner == user);
        if (dbFile == null)
            return BadRequest("Id not found or not owner.");

        var fileOnDisk = new FileInfo(Path.Combine(folderOptions.ProjectsDirectory ?? throw new ArgumentException("No Project Directory is set"), dbFile.Project.Id, fileName));

        if (fileOnDisk.Exists)
            fileOnDisk.Delete();
        
        dbContext.Files.Remove(dbFile);
        await dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index), new {id});
    }

    public async Task<IActionResult> Download(string id, string fileName)
    {
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(fileName))
            return BadRequest("No id given.");

        var user = await userManager.GetUserAsync(User) ?? throw new ArgumentException("User is null?");
        
        var dbFile = await dbContext.Files
            .Include(file => file.Project)
            .FirstOrDefaultAsync(f => f.Path == fileName && f.Project.Owner == user);
        if (dbFile == null)
            return BadRequest("Id not found or not owner.");

        var fileOnDisk = new FileInfo(Path.Combine(folderOptions.ProjectsDirectory ?? throw new ArgumentException("No Project Directory is set"), dbFile.Project.Id, fileName));

        if (!fileOnDisk.Exists)
        {
            dbContext.Files.Remove(dbFile);
            await dbContext.SaveChangesAsync();
            
            return BadRequest("File doesn't exist");
        }
        
        return File(fileOnDisk.OpenRead(), "application/octet-stream", fileOnDisk.Name);
    }
}