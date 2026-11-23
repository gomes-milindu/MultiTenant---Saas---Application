namespace WebApplication1.Service.Interface
{
    public interface IFileStorageService
    {
        Task<string> UploadFileAsync(IFormFile file, string folderPath);
        Task<string> UploadBytesAsync(byte[] fileBytes, string folderPath, string fileName, string contentType);
        Task DeleteFileByUrlAsync(string? fileUrl);
    }
}
