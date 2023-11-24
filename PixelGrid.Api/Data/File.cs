using System.ComponentModel.DataAnnotations;

namespace PixelGrid.Api.Data;

public class File(string projectId, string path, long size)
{
    [Key]
    public string Path { get; set; } = path;
    public long Size { get; set; } = size;
    public DateTime Uploaded { get; set; } = DateTime.UtcNow;

    public virtual Project Project { get; set; }
    public string ProjectId { get; set; } = projectId;

    public void UpdateFileSize(long fileSize)
    {
        Size = fileSize;
        Uploaded = DateTime.UtcNow;
    }
     
    // https://www.somacon.com/p576.php
    public string GetBytesReadable()
    {
        // Get absolute value
        var absoluteSize = Size < 0 ? -Size : Size;
        // Determine the suffix and readable value
        string suffix;
        double readable;
        switch (absoluteSize)
        {
            // Exabyte
            case >= 0x1000000000000000:
                suffix = "EB";
                readable = Size >> 50;
                break;
            // Petabyte
            case >= 0x4000000000000:
                suffix = "PB";
                readable = Size >> 40;
                break;
            // Terabyte
            case >= 0x10000000000:
                suffix = "TB";
                readable = Size >> 30;
                break;
            // Gigabyte
            case >= 0x40000000:
                suffix = "GB";
                readable = Size >> 20;
                break;
            // Megabyte
            case >= 0x100000:
                suffix = "MB";
                readable = Size >> 10;
                break;
            // Kilobyte
            case >= 0x400:
                suffix = "KB";
                readable = Size;
                break;
            default:
                return Size.ToString("0 B"); // Byte
        }
        
        // Divide by 1024 to get fractional value
        readable = readable / 1024;
        // Return formatted number with suffix
        return $"{readable:0.###} {suffix}";
    }
}