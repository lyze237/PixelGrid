namespace PixelGrid.Api.Managers;

public class ChunkManager
{
    private readonly Dictionary<string, List<Chunk>> files = new();
    
    /// <summary>
    /// Adds a chunk to the system.
    /// </summary>
    /// <exception cref="ArgumentException">Uuid is empty.</exception>
    public void StoreChunk(Chunk chunk)
    {
        if (string.IsNullOrWhiteSpace(chunk.Uuid))
            throw new ArgumentException("Invalid uuid");

        if (!files.ContainsKey(chunk.Uuid))
            files[chunk.Uuid] = new List<Chunk>();
        
        files[chunk.Uuid].Add(chunk);
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
        files[uuid].Max(c => c.ChunkIndex) + 1 == files[uuid].First().TotalChunkCount;

    public async Task Save(string basePath, string uuid)
    {
        var chunks = files[uuid];
        var metadataChunk = chunks.First();

        var file = new FileInfo(Path.Combine(basePath, metadataChunk.FullPath ?? metadataChunk.FileName));
        var fileDirectory = file.Directory;
        if (!fileDirectory?.Exists ?? false)
            fileDirectory.Create();
        
        await using var handle = file.Open(FileMode.Create, FileAccess.Write);
        
        foreach (var chunk in chunks.OrderBy(c => c.ChunkIndex))
            await handle.WriteAsync(chunk.Bytes);
    } 
}