namespace PixelGrid.Shared.Renderer.Exceptions;

public class OptionsException(string? message, string parameter) : Exception(message)
{
    public string Parameter { get; } = parameter;
}