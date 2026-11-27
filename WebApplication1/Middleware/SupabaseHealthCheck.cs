using System.Net.Http.Headers;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WebApplication1.Middleware;

public class SupabaseHealthCheck : IHealthCheck
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public SupabaseHealthCheck(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var url = _configuration["Supabase:Url"]?.Trim().TrimEnd('/');
        var key = _configuration["Supabase:ServiceRoleKey"]?.Trim();
        var bucket = _configuration["Supabase:BucketName"]?.Trim();

        if (string.IsNullOrWhiteSpace(url) || !Uri.TryCreate(url, UriKind.Absolute, out _))
            return HealthCheckResult.Unhealthy($"Invalid Supabase URL loaded: '{url ?? "NULL"}'. Ensure .env has Supabase__Url=https://...");

        if (string.IsNullOrWhiteSpace(key) || key == "SUPABASE SERVICE ROLE KEY")
            return HealthCheckResult.Unhealthy("Supabase ServiceRoleKey not loaded from .env.");

        if (string.IsNullOrWhiteSpace(bucket) || bucket == "SUPABASE BUCKET NAME")
            return HealthCheckResult.Unhealthy("Supabase BucketName not loaded from .env.");

        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(5);

        using var request = new HttpRequestMessage(HttpMethod.Get, $"{url}/storage/v1/bucket/{bucket}");
        request.Headers.Add("apikey", key);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", key);

        var response = await client.SendAsync(request, cancellationToken);

        return response.IsSuccessStatusCode
            ? HealthCheckResult.Healthy($"Supabase Storage bucket '{bucket}' is reachable.")
            : HealthCheckResult.Unhealthy($"Supabase returned HTTP {(int)response.StatusCode}.");
    }
}