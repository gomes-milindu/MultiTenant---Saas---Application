using System.Net.Sockets;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WebApplication1.Middleware;

public class EmailHealthCheck : IHealthCheck
{
    private readonly IConfiguration _configuration;

    public EmailHealthCheck(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        
        var host = Environment.GetEnvironmentVariable("SMTP_HOST") ?? _configuration["EmailSettings:SmtpServer"] ?? _configuration["EmailSettings:Host"];
        var portStr = Environment.GetEnvironmentVariable("SMTP_PORT") ?? _configuration["EmailSettings:Port"] ?? "587";

        if (string.IsNullOrWhiteSpace(host) || !int.TryParse(portStr, out var port))
            return HealthCheckResult.Unhealthy("SMTP Host or Port is not configured.");

        using var tcpClient = new TcpClient();
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(5));

        await tcpClient.ConnectAsync(host, port, timeoutCts.Token);

        return tcpClient.Connected ? HealthCheckResult.Healthy($"SMTP server ({host}:{port}) is reachable.") : HealthCheckResult.Unhealthy($"Unable to reach SMTP server ({host}:{port}).");
        
    }
}