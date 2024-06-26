namespace PixelGrid.Client.renderer.abstracts;

public interface IRenderCallback
{
    void OnStart();
    void OnProgress(CallbackStatus status);
    void OnWarning(string warning);
    void OnError(string error);
    void OnCompleted(int processExitCode);
    void OnLog(string line);
}