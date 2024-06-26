namespace PixelGrid.Client.Extensions;

public static class StringExtensions
{
    public static string AppendIfNeeded(this string str, string toAppend)
    {
        if (!str.EndsWith(toAppend))
            return str + toAppend;
        
        return str;
    }
}