namespace PixelGrid.Client.renderer.exceptions;

public class OptionsException(string? message, string parameter) : Exception(message)
{
    public string Parameter { get; } = parameter;
}