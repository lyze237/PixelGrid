using Microsoft.AspNetCore.SignalR.Client;

namespace PixelGrid.Client.Policies;

public class ReconnectRetryPolicy : IRetryPolicy
{
    private readonly Random random = new();
    
    public TimeSpan? NextRetryDelay(RetryContext retryContext)
    {
        if (retryContext.ElapsedTime < TimeSpan.FromSeconds(60))
            return TimeSpan.FromSeconds(random.Next(5, 10) * 1000);
        
        if (retryContext.ElapsedTime < TimeSpan.FromSeconds(300))
            return TimeSpan.FromSeconds(30);
        
        return TimeSpan.FromSeconds(60);
    }
}