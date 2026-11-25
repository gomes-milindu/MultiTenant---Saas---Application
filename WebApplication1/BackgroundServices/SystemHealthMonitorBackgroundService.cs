using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using WebApplication1.Data;
using WebApplication1.Models.Enums;
using WebApplication1.Service.Interface;


namespace WebApplication1.BackgroundServices;

public class SystemHealthMonitorBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly HealthCheckService _healthCheckService;
    private readonly ILogger<SystemHealthMonitorBackgroundService> _logger;

    private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(2); // every 2 minutes check the system Health
    private readonly TimeSpan _alertCooldown = TimeSpan.FromSeconds(5); // send email for same error every 30 minutes

    private DateTime? _lastAlertSentUtc;
    private bool _wasUnhealthy = false;

    public SystemHealthMonitorBackgroundService(IServiceScopeFactory scopeFactory, HealthCheckService healthCheckService, ILogger<SystemHealthMonitorBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _healthCheckService = healthCheckService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken); // after boot server starts monitoring after 15 seconds

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await EvaluateSystemHealthAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred inside SystemHealthMonitorBackgroundService.");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task EvaluateSystemHealthAsync(CancellationToken stoppingToken)
    {
        var report = await _healthCheckService.CheckHealthAsync(stoppingToken);
        var now = DateTime.UtcNow;

        if (report.Status != HealthStatus.Healthy)
        {
            _wasUnhealthy = true;

            if (_lastAlertSentUtc == null || (now - _lastAlertSentUtc.Value) >= _alertCooldown)
            {
                var failedDetails = string.Join("<br/>", report.Entries
                    .Where(e => e.Value.Status != HealthStatus.Healthy)
                    .Select(e =>
                        $"<li><b>{e.Key}</b>: {e.Value.Status} — {e.Value.Exception?.Message ?? e.Value.Description}</li>"));

                await NotifySuperAdminsAsync(
                    subject: $"SaaS System Health Critical - {report.Status}",
                    htmlBody: $@"
                        <h3>System Health Warning</h3>
                        <p>One or more system services became <b>{report.Status}</b> at <b>{now:yyyy-MM-dd HH:mm:ss} UTC</b>.</p>
                        <ul>{failedDetails}</ul>
                        <p>Please check your cloud database, external gateways, or server logs immediately.</p>",
                    stoppingToken
                );

                _lastAlertSentUtc = now;
            }
        }
        else if (_wasUnhealthy && report.Status == HealthStatus.Healthy)
        {
            _wasUnhealthy = false;
            _lastAlertSentUtc = null;

            await NotifySuperAdminsAsync(
                subject: "SaaS- [RECOVERED] All System Services Restored",
                htmlBody: $@"
                    <h3>System Back to Normal</h3>
                    <p>All monitored components (Database, Supabase, Email SMTP, SMS Gateway) are now <b>Healthy</b> as of <b>{now:yyyy-MM-dd HH:mm:ss} UTC</b>.</p>
                    <p>Total Health Check Latency: {Math.Round(report.TotalDuration.TotalMilliseconds, 2)} ms</p>",
                stoppingToken
            );
        }
    }

    private async Task NotifySuperAdminsAsync(string subject, string htmlBody, CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        var superAdminEmails = await dbContext.Users
            .Include(u => u.Profile)
            .Where(u => u.Role == UserRole.SuperAdmin && u.Status == AccountStatus.Active)
            .Select(u => u.Profile != null ? u.Profile.Email : null)
            .Where(email => !string.IsNullOrWhiteSpace(email))
            .Distinct()
            .ToListAsync(stoppingToken);

        foreach (var email in superAdminEmails)
        {
            try
            {
                await emailService.SendEmailAsync(email!, subject, htmlBody);
            }
            catch (Exception ex)
            {
         
                _logger.LogError(ex, "Failed to send health alert email to {Email}", email);
            }
        }
    }
}