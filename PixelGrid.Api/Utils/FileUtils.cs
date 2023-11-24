namespace PixelGrid.Api.Utils;

public static class FileUtils
{
    public static bool IsValidFileName(this IFormFile file) => 
        IsValidFileName(file.Name);

    public static bool IsValidFileName(this FileInfo file) => 
        IsValidFileName(file.Name);
    
    public static bool IsValidFileName(string fileName)
    {
        var invalidChars = new List<char>();
        invalidChars.AddRange(Path.GetInvalidPathChars());
        invalidChars.AddRange(Path.GetInvalidFileNameChars());

        return !fileName.Any(c => invalidChars.Contains(c));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <returns>True when folder is null or a valid relative path.</returns>
    public static bool IsValidRelativeFolderPath(string? path) => 
        string.IsNullOrWhiteSpace(path) || (!path.Contains("\\") && !path.Contains(":") && !path.Contains("/..") && !path.Contains("../") && !path.StartsWith("/"));

    public static bool IsDirectoryInside(this DirectoryInfo path, DirectoryInfo dir)
    {
        while (true)
        {
            if (path.Parent == null) 
                return false;

            if (string.Equals(path.Parent.FullName, dir.FullName, StringComparison.InvariantCultureIgnoreCase)) 
                return true;

            path = path.Parent;
        }
    }
}