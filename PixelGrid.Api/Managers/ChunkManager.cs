using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using PixelGrid.Api.Data;
using PixelGrid.Api.Options;

namespace PixelGrid.Api.Managers;

public class ChunkManager(IOptions<FolderOptions> folderOptions)
{
    private readonly FolderOptions folderOptions = folderOptions.Value;
    
    private readonly ConcurrentDictionary<string, List<Chunk>> files = new();

    /// <summary>
    /// Adds a chunk to the system.
    /// </summary>
    /// <exception cref="ArgumentException">Uuid is empty.</exception>
    public async Task StoreChunk(Chunk chunk, IFormFile file)
    {
        if (string.IsNullOrWhiteSpace(chunk.Uuid))
            throw new ArgumentException("Invalid uuid");
        
        await using var chunkStream = GetChunkFile(chunk).Open(FileMode.Create);
        await using var fileStream = file.OpenReadStream();
        await fileStream.CopyToAsync(chunkStream);
        
        files.GetOrAdd(chunk.Uuid, new List<Chunk>()).Add(chunk);
    }

    /// <summary>
    /// Checks if the chunk have modified metadata.
    /// </summary>
    /// <param name="uuid">The uuid of the file to check.</param>
    /// <returns>True if it's valid.</returns>
    public bool IsValid(string uuid)
    {
        var chunks = files[uuid];
        for (var i = 1; i < chunks.Count; i++)
        {
            if (!chunks[i - 1].SameMetadata(chunks[i]))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Checks if all chunks are uploaded.
    /// </summary>
    /// <param name="uuid">The uuid of the file to check.</param>
    /// <returns>True if all files have been uploaded.</returns>
    public bool IsComplete(string uuid) =>
        (ulong) files[uuid].LongCount() == files[uuid].First().TotalChunkCount;

    public async Task<long> Save(Project project, string uuid)
    {
        var chunks = files[uuid];
        var metadataChunk = chunks.First();
        
        var file = new FileInfo(Path.Combine(folderOptions.ProjectsDirectory?? throw new ArgumentException("Chunks directory not set"), project.Id, metadataChunk.FullPath ?? metadataChunk.FileName));
        if (!file.Directory?.Exists ?? false)
            file.Directory.Create();

        await using var finalFile = file.Open(FileMode.Create, FileAccess.Write);

        foreach (var chunk in chunks.OrderBy(c => c.ChunkIndex))
        {
            await using var chunkFile = GetChunkFile(chunk).OpenRead();
            await chunkFile.CopyToAsync(finalFile);
        }
        
        GetChunkFolder(metadataChunk).Delete(true);

        return finalFile.Length;
    }
    
    private FileInfo GetChunkFile(Chunk chunk)
    {
        var folder = GetChunkFolder(chunk);

        var file = new FileInfo(Path.Combine(folder.FullName, $"{chunk.ChunkIndex}.part"));
        return file;
    }

    private DirectoryInfo GetChunkFolder(Chunk chunk)
    {
        var folder = new DirectoryInfo(Path.Combine(folderOptions.ChunksDirectory ?? throw new ArgumentException("Chunks directory not set"), chunk.Uuid));
        if (!folder.Exists)
            folder.Create();
        
        return folder;
    }
}