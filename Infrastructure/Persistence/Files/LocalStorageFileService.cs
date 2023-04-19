using FilmFreakApi.Infrastructure.Configuration.Models;
using Microsoft.Extensions.Configuration;

namespace FilmFreakApi.Infrastructure.Persistence.Files;

public class LocalStorageFileService : IFileUploadService
{
    private readonly IConfiguration _configuration;

    public LocalStorageFileService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task DeleteFile(string fileId)
    {
        throw new NotImplementedException();
    }

    public async Task<string> StoreFile(Stream file)
    {
        var fileUploadOptions = _configuration
            .GetSection(SectionNames.FileUpload)
            .Get<FileUploadOptions>();

        if (fileUploadOptions == null)
        {
            throw new Exception($"Missing {SectionNames.FileUpload} configuration.");
        }

        if (string.IsNullOrEmpty(fileUploadOptions.Folder))
        {
            throw new Exception($"Folder setting not set in ${SectionNames.FileUpload}");
        }

        var releaseFilesPath = Path.Combine(fileUploadOptions.Folder, "Release");
        Directory.CreateDirectory(releaseFilesPath);

        var fileId = Guid.NewGuid().ToString();
        var filePath = Path.Combine(releaseFilesPath, fileId);

        using var stream = System.IO.File.Create(filePath);
        await file.CopyToAsync(stream);

        return fileId;
    }
}