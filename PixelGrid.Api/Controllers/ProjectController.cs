using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PixelGrid.Api.Data;
using PixelGrid.Api.Options;
using PixelGrid.Api.Utils;

namespace PixelGrid.Api.Controllers;

public record ProjectIndexModel(List<Project> OwnProjects, List<Project> SharedProjects);

public record ProjectEditModel(Project Project, List<string> Files);

[Authorize]
public class ProjectController(ApplicationDbContext dbContext, UserManager<User> userManager,
    IOptions<FolderOptions> folderOptions, ILogger<ProjectController> logger) : Controller
{
    private readonly FolderOptions folderOptions = folderOptions.Value;

    public async Task<IActionResult> Index()
    {
        var user = await userManager.GetUserAsync(User) ?? throw new ArgumentException("User is null?");

        var model = new ProjectIndexModel(
            await dbContext.Projects
                .Include(c => c.SharedWith)
                .Where(c => c.Owner == user)
                .ToListAsync(),
            await dbContext.Projects
                .Include(c => c.Owner)
                .Where(c => c.SharedWith.Contains(user))
                .ToListAsync()
        );
        return View(model);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest($"{nameof(name)} is not set.");

        var user = await userManager.GetUserAsync(User) ?? throw new ArgumentException("User is null?");

        dbContext.Projects.Add(new Project(name, user.Id));
        await dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("No id given.");

        var user = await userManager.GetUserAsync(User) ?? throw new ArgumentException("User is null?");
        var project = await dbContext.Projects
            .Include(c => c.SharedWith)
            .FirstOrDefaultAsync(c => c.Id == id && c.Owner == user);

        if (project == null)
            return BadRequest("Id not found or not owner.");

        var files = new List<string>();
        var folder = new DirectoryInfo(Path.Combine(
            folderOptions.ProjectsDirectory ?? throw new ArgumentException("No Project Directory is set"), project.Id));
        if (folder.Exists)
        {
            files.AddRange(folder
                .EnumerateFiles("*.*", SearchOption.AllDirectories)
                .Select(f => Path.GetRelativePath(folder.FullName, f.FullName).Replace("\\", "/")));
        }

        return View(new ProjectEditModel(project, files));
    }

    public async Task<IActionResult> Delete(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("No id given.");

        var user = await userManager.GetUserAsync(User) ?? throw new ArgumentException("User is null?");
        var project = await dbContext.Projects
            .Include(c => c.SharedWith)
            .FirstOrDefaultAsync(c => c.Id == id && c.Owner == user);

        if (project == null)
            return BadRequest("Id not found or not owner.");

        dbContext.Projects.Remove(project);
        await dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [RequestSizeLimit(5000000000)]
    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file, [FromForm] string dzfullpath, [FromForm] string projectId)
    {
        if (string.IsNullOrWhiteSpace(projectId))
            return BadRequest("No id given.");

        var user = await userManager.GetUserAsync(User) ?? throw new ArgumentException("User is null?");
        var project = await dbContext.Projects
            .Include(c => c.SharedWith)
            .FirstOrDefaultAsync(c => c.Id == projectId && c.Owner == user);

        if (project == null)
            return BadRequest("Id not found or not owner.");

        var folder = new DirectoryInfo(Path.Combine(
            folderOptions.ProjectsDirectory ?? throw new ArgumentException("No Project Directory is set"), project.Id));

        if (!file.IsValidFileName())
            return BadRequest("Invalid filename");

        if (!FileUtils.IsValidRelativeFolderPath(dzfullpath))
            return BadRequest("Invalid path");

        var fileOnDisk = new FileInfo(Path.Combine(folder.FullName, dzfullpath));
        var fileOnDiskDirectory = fileOnDisk.Directory;
        if (fileOnDiskDirectory == null)
            return BadRequest("Unknown directory");

        if (!fileOnDiskDirectory.IsDirectoryInside(folder))
            return BadRequest("Nope");

        if (!fileOnDiskDirectory.Exists)
            fileOnDiskDirectory.Create();

        await using var fileOnDiskStream = fileOnDisk.Open(FileMode.Create);

        await file.CopyToAsync(fileOnDiskStream);

        return Ok();
    }

    public async Task<IActionResult> DeleteFile(string id, string fileName)
    {
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(fileName))
            return BadRequest("No id given.");

        var user = await userManager.GetUserAsync(User) ?? throw new ArgumentException("User is null?");
        var project = await dbContext.Projects
            .Include(c => c.SharedWith)
            .FirstOrDefaultAsync(c => c.Id == id && c.Owner == user);
        
        if (project == null)
            return BadRequest("Id not found or not owner.");

        var folder = new DirectoryInfo(Path.Combine(
            folderOptions.ProjectsDirectory ?? throw new ArgumentException("No Project Directory is set"), project.Id));

        if (!FileUtils.IsValidFileName(Path.GetFileName(fileName)))
            return BadRequest("Invalid filename");

        if (!FileUtils.IsValidRelativeFolderPath(fileName))
            return BadRequest("Invalid path");

        var fileOnDisk = new FileInfo(Path.Combine(folder.FullName, fileName));
        var fileOnDiskDirectory = fileOnDisk.Directory;
        if (fileOnDiskDirectory == null)
            return BadRequest("Unknown directory");

        if (!fileOnDiskDirectory.IsDirectoryInside(folder))
            return BadRequest("Nope");

        if (!fileOnDisk.Exists)
            return BadRequest("File doesn't exist");

        fileOnDisk.Delete();

        return RedirectToAction(nameof(Edit), new {id});
    }

    public async Task<IActionResult> DownloadFile(string id, string fileName)
    {
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(fileName))
            return BadRequest("No id given.");

        var user = await userManager.GetUserAsync(User) ?? throw new ArgumentException("User is null?");
        var project = await dbContext.Projects
            .Include(c => c.SharedWith)
            .FirstOrDefaultAsync(c => c.Id == id && c.Owner == user);
        
        if (project == null)
            return BadRequest("Id not found or not owner.");

        var folder = new DirectoryInfo(Path.Combine(
            folderOptions.ProjectsDirectory ?? throw new ArgumentException("No Project Directory is set"), project.Id));

        if (!FileUtils.IsValidFileName(Path.GetFileName(fileName)))
            return BadRequest("Invalid filename");

        if (!FileUtils.IsValidRelativeFolderPath(fileName))
            return BadRequest("Invalid path");

        var fileOnDisk = new FileInfo(Path.Combine(folder.FullName, fileName));
        var fileOnDiskDirectory = fileOnDisk.Directory;
        if (fileOnDiskDirectory == null)
            return BadRequest("Unknown directory");

        if (!fileOnDiskDirectory.IsDirectoryInside(folder))
            return BadRequest("Nope");

        if (!fileOnDisk.Exists)
            return BadRequest("File doesn't exist");

        return File(fileOnDisk.OpenRead(), "application/octet-stream", fileOnDisk.Name);
    }
}