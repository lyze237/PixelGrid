namespace PixelGrid.Server.Infra.Exceptions;

public class EntityNotFoundException<TEntity>(string? message) : Exception(message)
{
    public Type Type => typeof(TEntity);
}