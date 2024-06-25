namespace PixelGrid.Renderer.abstracts;

public interface IRenderCallback
{
    void OnStart();
    void OnProgress(CallbackStatus status);
    void OnWarning(string warning);
    void OnError(string error);
    void OnCompleted();
    void OnLog(string line);
}