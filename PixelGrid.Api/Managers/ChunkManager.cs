namespace PixelGrid.Api.Managers;

public class ChunkManager
{
    private readonly Dictionary<string, List<Chunk>> chunks = new();
    public void StoreChunk(Chunk chunk)
    {
        if (string.IsNullOrWhiteSpace(chunk.Uuid))
            throw new ArgumentException("Invalid uuid");

        if (!chunks.ContainsKey(chunk.Uuid))
            chunks[chunk.Uuid] = new List<Chunk>();
        
        chunks[chunk.Uuid].Add(chunk);
    }
}