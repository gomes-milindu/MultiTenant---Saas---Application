using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebApplication1.Data;
using WebApplication1.Service.Interface;


namespace WebApplication1.BackgroundServices;

public class PackageReminderBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PackageReminderBackgroundService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(30); // Testing one hour

    public PackageReminderBackgroundService(IServiceScopeFactory scopeFactory, ILogger<PackageReminderBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Package Reminder Background Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPackageRemindersAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing package expiration reminders.");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("Package Reminder Background Service is stopping.");
    }

    private async Task ProcessPackageRemindersAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        var settings = await dbContext.PackageReminderSettings.FirstOrDefaultAsync();
        if (settings == null || !settings.IsEnabled)
        {
            return; 
        }


        HashSet<Guid> targetTeacherIds = new();

        if (settings.TargetScope == "SPECIFIC")
        {
            var ids = await dbContext.PackageReminderTargetTeachers.Select(t => t.TeacherUserId).ToListAsync();

            targetTeacherIds = ids.ToHashSet();

            if (!targetTeacherIds.Any())
            {
                return; 
            }
        }

        var now = DateTime.UtcNow;
        bool hasChanges = false;

        var activePackagesQuery = dbContext.TeacherPackages
            .Include(tp => tp.User)
            .ThenInclude(u => u.Profile)
            .Include(tp => tp.Package)
            .Where(tp => tp.Status == "ACTIVE");

        if (settings.TargetScope == "SPECIFIC")
        {
            activePackagesQuery = activePackagesQuery.Where(tp => targetTeacherIds.Contains(tp.UserId));
        }

        var activePackages = await activePackagesQuery.ToListAsync();

        foreach (var pkg in activePackages)
        {
            if (pkg.User == null || string.IsNullOrWhiteSpace(pkg.User.Profile?.Email))
                continue;

            var remainingDays = (pkg.ExpiresAt - now).TotalDays;

            // A. පැකේජය දැනටමත් Expire වී ඇත්නම් (ExpiresAt <= now)
            if (pkg.ExpiresAt <= now)
            {
                pkg.Status = "EXPIRED";
                hasChanges = true;

                if (settings.EnableExpirationEmail && !pkg.ExpirationEmailSent)
                {
                    string subject = $"Your {pkg.Package?.Name ?? "Teaching"} Package Has Expired";
                    string body =
                        $"Dear {pkg.User.Username},\n\n" +
                        $"Your subscription package ({pkg.Package?.Name}) expired on {pkg.ExpiresAt:yyyy-MM-dd HH:mm} UTC.\n" +
                        $"Please renew your package to continue creating classes and managing students without interruption.\n\n" +
                        $"Thank you!";

                    await emailService.SendEmailAsync(pkg.User.Profile?.Email, subject, body);
                    pkg.ExpirationEmailSent = true;
                }

                continue; 
            }

            // second reminder

            if (settings.EnableSecondReminder && !pkg.SecondReminderSent && remainingDays <= settings.SecondReminderDaysBefore)
            {
                int daysLeft = Math.Max(1, (int)Math.Ceiling(remainingDays));
                string subject = $"Final Reminder: Your Package Expires in {daysLeft} Day(s)!";
                string body =
                    $"Dear {pkg.User.Username},\n\n" +
                    $"This is a final reminder that your active package ({pkg.Package?.Name}) will expire in {daysLeft} day(s) on {pkg.ExpiresAt:yyyy-MM-dd}.\n" +
                    $"Please renew your subscription soon to avoid service interruption.\n\n" +
                    $"Thank you!";

                await emailService.SendEmailAsync(pkg.User.Profile?.Email, subject, body);

                pkg.SecondReminderSent = true;
                pkg.FirstReminderSent = true;
                hasChanges = true;
            }
            
            else if (settings.EnableFirstReminder && !pkg.FirstReminderSent &&  remainingDays <= settings.FirstReminderDaysBefore)
            {
                int daysLeft = Math.Max(1, (int)Math.Ceiling(remainingDays));
                string subject = $"Reminder: Your Package Expires in {daysLeft} Day(s)";
                string body =
                    $"Dear {pkg.User.Username},\n\n" +
                    $"Your active teaching package ({pkg.Package?.Name}) is scheduled to expire in {daysLeft} day(s) on {pkg.ExpiresAt:yyyy-MM-dd}.\n" +
                    $"You can renew or upgrade your package anytime from your dashboard.\n\n" +
                    $"Thank you!";

                await emailService.SendEmailAsync(pkg.User.Profile?.Email, subject, body);

                pkg.FirstReminderSent = true;
                hasChanges = true;
            }
        }

        if (hasChanges)
        {
            await dbContext.SaveChangesAsync();
            _logger.LogInformation("Processed package reminder emails and status updates successfully.");
        }
    }
}