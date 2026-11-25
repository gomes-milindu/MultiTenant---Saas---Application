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
        
        var url = (Environment.GetEnvironmentVariable("SUPERBASE__URL") ?? _configuration["Supabase:Url"])?.TrimEnd('/');
        var key = Environment.GetEnvironmentVariable("SUPERBASE__KEY") ?? _configuration["Supabase:ServiceRoleKey"];
        var bucket = Environment.GetEnvironmentVariable("BUCKET__NAME") ?? _configuration["Supabase:BucketName"];

        if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(bucket))
            return HealthCheckResult.Unhealthy("Supabase credentials or bucket name missing in .env/configuration.");

        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(5);

        using var request = new HttpRequestMessage(HttpMethod.Get, $"{url}/storage/v1/bucket/{bucket}");
        request.Headers.Add("apikey", key);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", key);

        var response = await client.SendAsync(request, cancellationToken);
            
        return response.IsSuccessStatusCode ? HealthCheckResult.Healthy($"Supabase Storage bucket '{bucket}' is reachable.") : HealthCheckResult.Unhealthy($"Supabase returned HTTP {(int)response.StatusCode}.");
        
    }
}