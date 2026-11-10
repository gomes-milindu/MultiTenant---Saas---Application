using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data; 

namespace WebApplication1.BackgroundServices;

public class PackageExpirationBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly ILogger<PackageExpirationBackgroundService> logger;

    private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(5);

    public PackageExpirationBackgroundService(IServiceScopeFactory scopeFactory, ILogger<PackageExpirationBackgroundService> logger)
    {
        this.scopeFactory = scopeFactory;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Package Expiration Background Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckAndExpirePackagesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while updating expired teacher packages.");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        logger.LogInformation("Package Expiration Background Service is stopping.");
    }

    private async Task CheckAndExpirePackagesAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var now = DateTime.UtcNow;

        var expiredPackages = await dbContext.TeacherPackages
            .Where(tp => tp.Status == "ACTIVE" && tp.ExpiresAt <= now)
            .ToListAsync();

        if (expiredPackages.Any())
        {
            foreach (var package in expiredPackages)
            {
                package.Status = "EXPIRED";
            }

            await dbContext.SaveChangesAsync();
            logger.LogInformation($"Successfully updated {expiredPackages.Count} expired teacher package(s) to 'EXPIRED'.");
        }
    }
}