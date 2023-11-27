namespace PixelGrid.Api.Managers;

public class Chunk(string dzuuid, ulong chunkIndex, ulong totalFileSize, ulong chunkSize, ulong totalChunkCount, ulong chunkByteOffset, string? fullPath, string fileName)
{
    public string Uuid { get; } = dzuuid;
    public ulong ChunkIndex { get; } = chunkIndex;
    public ulong TotalFileSize { get; } = totalFileSize;
    public ulong ChunkSize { get; } = chunkSize;
    public ulong TotalChunkCount { get; } = totalChunkCount;
    public ulong ChunkByteOffset { get; init; } = chunkByteOffset;
    public string? FullPath { get; } = fullPath;
    public string FileName { get; } = fileName;

    public bool ValidFileName()
    {
        var invalidChars = new List<char>();
        invalidChars.AddRange(Path.GetInvalidPathChars());
        invalidChars.AddRange(Path.GetInvalidFileNameChars());

        return FileName.All(c => !invalidChars.Contains(c));
    }

    public bool ValidFilePath() =>
        string.IsNullOrWhiteSpace(FullPath) || (!FullPath.Contains("\\") && !FullPath.Contains(":"));

    public bool SameMetadata(Chunk other) =>
        Uuid == other.Uuid && TotalFileSize == other.TotalFileSize && ChunkSize == other.ChunkSize &&
        TotalChunkCount == other.TotalChunkCount && FullPath == other.FullPath && FileName == other.FileName;

    public override string ToString()
    {
        return $"{nameof(Uuid)}: {Uuid}, {nameof(ChunkIndex)}: {ChunkIndex}, {nameof(TotalFileSize)}: {TotalFileSize}, {nameof(ChunkSize)}: {ChunkSize}, {nameof(TotalChunkCount)}: {TotalChunkCount}, {nameof(ChunkByteOffset)}: {ChunkByteOffset}, {nameof(FullPath)}: {FullPath}, {nameof(FileName)}: {FileName}";
    }
}