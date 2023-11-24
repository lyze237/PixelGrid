using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using PixelGrid.Api.Helpers;

namespace PixelGrid.Api.Managers;

public class Chunk
{
    public string Uuid { get; set; }
    public ulong ChunkIndex { get; set; }
    public ulong TotalFileSize { get; set; }
    public ulong ChunkSize { get; set; }
    public ulong TotalChunkCount { get; set; }
    public ulong ChunkByteOffset { get; set; }
    public string? FullPath { get; set; }
    public string FileName { get; set; }
    public byte[] Bytes { get; set; }

    public async Task Parse(MultipartReader reader)
    {
        var section = await reader.ReadNextSectionAsync();
        while (section != null)
        {
            var hasContentDispositionHeader =
                ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);
            if (!hasContentDispositionHeader)
                continue;

            if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                await ParseFileData(section.AsFileSection()!);
            else if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition))
                await ParseFormData(section.AsFormDataSection()!);

            section = await reader.ReadNextSectionAsync();
        }
    }

    private async Task ParseFileData(FileMultipartSection file)
    {
        using var memory = new MemoryStream();
        await file.FileStream!.CopyToAsync(memory);

        FileName = file.FileName;
        Bytes = memory.ToArray();
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private async Task ParseFormData(FormMultipartSection form)
    {
        switch (form.Name.ToLower())
        {
            case "dzuuid":
                Uuid = await form.GetValueAsync();
                break;
            case "dzchunkindex":
                ChunkIndex = Convert.ToUInt64(await form.GetValueAsync());
                break;
            case "dztotalfilesize":
                TotalFileSize = Convert.ToUInt64(await form.GetValueAsync());
                break;
            case "dzchunksize":
                ChunkSize = Convert.ToUInt64(await form.GetValueAsync());
                break;
            case "dztotalchunkcount":
                TotalChunkCount = Convert.ToUInt64(await form.GetValueAsync());
                break;
            case "dzchunkbyteoffset":
                ChunkByteOffset = Convert.ToUInt64(await form.GetValueAsync());
                break;
            case "dzfullpath":
                FullPath = await form.GetValueAsync();
                break;
        }
    }

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
}