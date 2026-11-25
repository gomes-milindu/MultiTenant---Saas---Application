using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WebApplication1.Middleware;

public class SmsHealthCheck : IHealthCheck
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public SmsHealthCheck(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        
        var gatewayUrl = Environment.GetEnvironmentVariable("QUICKSEND_URL") ?? _configuration["SmsSettings:GatewayUrl"];
        var apiKey = Environment.GetEnvironmentVariable("QUICKSEND_API_KEY") ?? _configuration["SmsSettings:ApiKey"];

        if (string.IsNullOrWhiteSpace(gatewayUrl) || gatewayUrl == "QUICKSEND URL" || string.IsNullOrWhiteSpace(apiKey) || apiKey == "QUICKSEND API KEY")
        {
            return HealthCheckResult.Unhealthy("QuickSend SMS Gateway URL or API Key is not configured.");
        }

        var uri = new Uri(gatewayUrl);
        var baseHostUrl = $"{uri.Scheme}://{uri.Host}";

        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(5);

        using var response = await client.GetAsync(baseHostUrl, cancellationToken);

        return HealthCheckResult.Healthy($"SMS Gateway ({uri.Host}) is reachable (HTTP {(int)response.StatusCode}).");
        
    }
}