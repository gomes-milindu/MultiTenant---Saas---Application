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
        var gatewayUrl = _configuration["SmsSettings:GatewayUrl"]?.Trim();
        var apiKey = _configuration["SmsSettings:ApiKey"]?.Trim();
        var senderId = _configuration["SmsSettings:SenderId"]?.Trim();
        var email = _configuration["SmsSettings:Email"]?.Trim();

        if (string.IsNullOrWhiteSpace(gatewayUrl) || gatewayUrl == "QUICKSEND URL" || !Uri.TryCreate(gatewayUrl, UriKind.Absolute, out var uri))
        {
            return HealthCheckResult.Unhealthy($"Invalid SMS Gateway URL loaded: '{gatewayUrl ?? "NULL"}'. Ensure .env has SmsSettings__GatewayUrl=https://...");
        }

        if (string.IsNullOrWhiteSpace(apiKey) || apiKey == "QUICKSEND API KEY" ||
            string.IsNullOrWhiteSpace(senderId) || senderId == "QUICKSEND SENDER ID" ||
            string.IsNullOrWhiteSpace(email) || email == "QUICKSEND EMAIL")
        {
            return HealthCheckResult.Unhealthy("QuickSend SMS credentials (ApiKey, SenderId, or Email) not loaded from .env.");
        }

        var baseHostUrl = $"{uri.Scheme}://{uri.Host}";

        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(5);

        using var response = await client.GetAsync(baseHostUrl, cancellationToken);

        return HealthCheckResult.Healthy($"SMS Gateway ({uri.Host}) is reachable (HTTP {(int)response.StatusCode}).");
    }
}