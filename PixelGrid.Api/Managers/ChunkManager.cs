namespace PixelGrid.Api.Managers;

public class ChunkManager
{
    private readonly Dictionary<string, List<Chunk>> chunks = new();
    
    /// <summary>
    /// Adds a chunk to the system.
    /// </summary>
    /// <exception cref="ArgumentException">Uuid is empty.</exception>
    public void StoreChunk(Chunk chunk)
    {
        if (string.IsNullOrWhiteSpace(chunk.Uuid))
            throw new ArgumentException("Invalid uuid");

        if (!chunks.ContainsKey(chunk.Uuid))
            chunks[chunk.Uuid] = new List<Chunk>();
        
        chunks[chunk.Uuid].Add(chunk);
    }

    /// <summary>
    /// Checks if the chunk has garbled TotalChunkCount.
    /// Todo: Add more checks
    /// </summary>
    /// <param name="uuid">The uuid of the file to check.</param>
    /// <returns>True if it's valid.</returns>
    public bool IsValid(string uuid) => 
        chunks[uuid].GroupBy(c => c.TotalChunkCount).Count() == 1;

    /// <summary>
    /// Checks if all chunks are uploaded.
    /// </summary>
    /// <param name="uuid">The uuid of the file to check.</param>
    /// <returns>True if all files have been uploaded.</returns>
    public bool IsComplete(string uuid) => 
        chunks[uuid].Max(c => c.ChunkIndex) + 1 == chunks[uuid].First().TotalChunkCount;
}