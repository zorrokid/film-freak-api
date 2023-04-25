using FilmFreakApi.RestApi.Validators;
using Microsoft.AspNetCore.Http;

public class FileValidatorTests
{
    private static byte[] _jpegFileData = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0xFF, 0xFF, 0xFF };
    private static byte[] _unknownFileData = new byte[] { 0x01 };

    private const string _jpegContentType = "image/jpeg";
    private const string _jpegFileName = "file.jpeg";

    [Fact]
    public void Validate_FileMatchesTheSpec_ValidationPassesWithoutExceptions()
    {
        using var memoryStream = new MemoryStream(_jpegFileData);
        var formFile = new FormFile(memoryStream, 0, _jpegFileData.Length, _jpegFileName, _jpegFileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = _jpegContentType
        };
        var validator = new ImageFileValidator(FileValidationSpecsBuilder.Build(FileUploadType.Jpeg));
        validator.Validate(formFile);
    }

    [Fact]
    public void Validate_FileHasNotMatchingSignature_ThrowsFileValidationoException()
    {
        using var memoryStream = new MemoryStream(_unknownFileData);
        var formFile = new FormFile(memoryStream, 0, _unknownFileData.Length, _jpegFileName, _jpegFileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = _jpegContentType
        };
        var validator = new ImageFileValidator(FileValidationSpecsBuilder.Build(FileUploadType.Jpeg));
        Assert.Throws<FileValidationException>(() => validator.Validate(formFile));
    }

    [Fact]
    public void Validate_FileHasNotMatchingContentType_ThrowsFileValidationoException()
    {
        using var memoryStream = new MemoryStream(_jpegFileData);
        var formFile = new FormFile(memoryStream, 0, _jpegFileData.Length, _jpegFileName, _jpegFileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/xxx"
        };
        var validator = new ImageFileValidator(FileValidationSpecsBuilder.Build(FileUploadType.Jpeg));
        Assert.Throws<FileValidationException>(() => validator.Validate(formFile));
    }

    [Fact]
    public void Validate_FileHasNotMatchingFileName_ThrowsFileValidationoException()
    {
        using var memoryStream = new MemoryStream(_jpegFileData);
        var formFile = new FormFile(memoryStream, 0, _jpegFileData.Length, _jpegFileName, "file.xxx")
        {
            Headers = new HeaderDictionary(),
            ContentType = _jpegContentType
        };
        var validator = new ImageFileValidator(FileValidationSpecsBuilder.Build(FileUploadType.Jpeg));
        Assert.Throws<FileValidationException>(() => validator.Validate(formFile));
    }
}