using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StorageTestController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public StorageTestController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("test-connection")]
    public async Task<IActionResult> TestConnection()
    {
        var supabaseUrl = _configuration["Supabase:Url"]?.TrimEnd('/');
        var serviceRoleKey = _configuration["Supabase:ServiceRoleKey"];
        var bucketName = _configuration["Supabase:BucketName"];

        if (string.IsNullOrWhiteSpace(supabaseUrl) || string.IsNullOrWhiteSpace(serviceRoleKey))
            return BadRequest(new { message = "appsettings.json හි Supabase Url හෝ ServiceRoleKey නොමැත." });

        var fileName = $"ping-{Guid.NewGuid()}.txt";
        var objectPath = $"connection-tests/{fileName}";
        var uploadUrl = $"{supabaseUrl}/storage/v1/object/{bucketName}/{objectPath}";

        using var httpClient = new HttpClient();
        using var request = new HttpRequestMessage(HttpMethod.Post, uploadUrl);

        
        request.Headers.Add("apikey", serviceRoleKey);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", serviceRoleKey);
        request.Headers.Add("x-upsert", "true");

        var content = new ByteArrayContent(Encoding.UTF8.GetBytes($"Connected at {DateTime.UtcNow}"));
        content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
        request.Content = content;

        var response = await httpClient.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, new
            {
                connected = false,
                statusCode = response.StatusCode,
                error = responseBody
            });
        }

        var publicUrl = $"{supabaseUrl}/storage/v1/object/public/{bucketName}/{objectPath}";

        return Ok(new
        {
            connected = true,
            message = "Supabase Storage connected successfully!",
            publicUrl
        });
    }
}