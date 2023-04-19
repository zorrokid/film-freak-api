public interface IFileUploadService
{
    Task<string> StoreFile(Stream file);
    Task DeleteFile(string fileId);
}