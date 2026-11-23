using System.Net.Http.Headers;
using WebApplication1.Service.Interface;

namespace WebApplication1.Service.Storage.Impl;

public class SupabaseStorageService : IFileStorageService
{
    private readonly HttpClient _httpClient;
    private readonly string _supabaseUrl;
    private readonly string _serviceRoleKey;
    private readonly string _bucketName;

    public SupabaseStorageService(HttpClient httpClient)
    {
        _httpClient = httpClient;

        var rawUrl = Environment.GetEnvironmentVariable("SUPERBASE__URL");
        if (string.IsNullOrWhiteSpace(rawUrl) || !Uri.TryCreate(rawUrl, UriKind.Absolute, out _))
        {
            throw new InvalidOperationException(
                $"SUPERBASE__URL is missing or invalid in .env file. Current value: '{rawUrl}'");
        }

        _supabaseUrl = rawUrl.TrimEnd('/');
        _serviceRoleKey = Environment.GetEnvironmentVariable("SUPERBASE__KEY")
            ?? throw new InvalidOperationException("SUPERBASE__KEY is not configured in .env file.");
        _bucketName = Environment.GetEnvironmentVariable("BUCKET__NAME")
            ?? throw new InvalidOperationException("BUCKET__NAME is not configured in .env file.");
    }

    public async Task<string> UploadFileAsync(IFormFile file, string folderPath)
    {
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        var bytes = memoryStream.ToArray();

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";

        return await UploadBytesAsync(bytes, folderPath, uniqueFileName, file.ContentType);
    }

    public async Task<string> UploadBytesAsync(byte[] fileBytes, string folderPath, string fileName, string contentType)
    {
        var objectPath = $"{folderPath.Trim('/')}/{fileName}";
        var uploadEndpoint = $"{_supabaseUrl}/storage/v1/object/{_bucketName}/{objectPath}";

        using var request = new HttpRequestMessage(HttpMethod.Post, uploadEndpoint);
        request.Headers.Add("apikey", _serviceRoleKey);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _serviceRoleKey);
        request.Headers.Add("x-upsert", "true");

        var content = new ByteArrayContent(fileBytes);
        content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        request.Content = content;

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Supabase storage upload failed: {errorBody}");
        }

        return $"{_supabaseUrl}/storage/v1/object/public/{_bucketName}/{objectPath}";
    }

    public async Task DeleteFileByUrlAsync(string? fileUrl)
    {
        if (string.IsNullOrWhiteSpace(fileUrl)) return;

        var publicPrefix = $"{_supabaseUrl}/storage/v1/object/public/{_bucketName}/";
        if (!fileUrl.StartsWith(publicPrefix)) return;

        var objectPath = fileUrl.Substring(publicPrefix.Length);
        var deleteEndpoint = $"{_supabaseUrl}/storage/v1/object/{_bucketName}/{objectPath}";

        using var request = new HttpRequestMessage(HttpMethod.Delete, deleteEndpoint);
        request.Headers.Add("apikey", _serviceRoleKey);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _serviceRoleKey);

        await _httpClient.SendAsync(request);
    }
}