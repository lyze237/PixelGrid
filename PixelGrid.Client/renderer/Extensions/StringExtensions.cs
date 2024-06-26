namespace PixelGrid.Client.renderer.Extensions;

public static class StringExtensions
{
    public static int? ToInt(this string str)
    {
        if (int.TryParse(str, out var res))
            return res;

        return null;
    }
    
    public static long? ToLong(this string str)
    {
        if (long.TryParse(str, out var res))
            return res;

        return null;
    }
    
    public static float? ToFloat(this string str)
    {
        if (float.TryParse(str, out var res))
            return res;

        return null;
    }
}