using Infrastructure.Database;

using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Functions.Administration;

public class TokenCleanup(ApplicationDbContext dbContext, ILogger<TokenCleanup> logger)
{
    [Function(nameof(TokenCleanup))]
    public async Task Run([TimerTrigger("0 0 */1 * * *"
#if DEBUG
            , RunOnStartup = true
#endif
        )]
        TimerInfo timer)
    {
        if (logger.IsEnabled(LogLevel.Debug))
            logger.LogDebug("TokenCleanup triggered. ScheduleStatus: Last={Last}, Next={Next}, IsPastDue={IsPastDue}", timer.ScheduleStatus?.Last, timer.ScheduleStatus?.Next, timer.IsPastDue);

        try
        {
            if (logger.IsEnabled(LogLevel.Debug)) 
                logger.LogDebug("Starting expired refresh token cleanup...");
            
            var deleted = await dbContext.RefreshTokens
                .Where(t => t.ExpiresAtUtc < DateTime.UtcNow)
                .ExecuteDeleteAsync();
            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation("Token cleanup completed. Removed {Count} expired tokens.", deleted);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Token cleanup failed with an exception.");
            throw;
        }
    }
}