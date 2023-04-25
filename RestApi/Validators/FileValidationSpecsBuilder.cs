namespace FilmFreakApi.RestApi.Validators;

public enum FileUploadType
{
    Jpeg
};

public static class FileValidationSpecsBuilder
{
    private static readonly Dictionary<FileUploadType, string> _contentTypes = new Dictionary<FileUploadType, string>
    {
        { FileUploadType.Jpeg, "image/jpeg" },
    };

    private static readonly Dictionary<FileUploadType, List<string>> _extensions = new Dictionary<FileUploadType, List<string>>
    {
        { FileUploadType.Jpeg, new List<string> { ".jpg", ".jpeg" }},
    };

    private static readonly Dictionary<FileUploadType, List<byte[]>> _signatures = new Dictionary<FileUploadType, List<byte[]>>
    {
        {
            FileUploadType.Jpeg, new List<byte[]>
            {
                new byte[] {0xFF, 0xD8, 0xFF, 0xE0},
                new byte[] {0xFF, 0xD8, 0xFF, 0xE2},
                new byte[] {0xFF, 0xD8, 0xFF, 0xE3}
            }
        }
    };

    public static FileValidationSpecs Build(FileUploadType fileUploadType)
    {
        return new FileValidationSpecs(
             ContentType: _contentTypes[fileUploadType],
             Extensions: _extensions[fileUploadType],
             MaxSizeInBytes: 1024,
             Signature: _signatures[fileUploadType]
         );
    }
}