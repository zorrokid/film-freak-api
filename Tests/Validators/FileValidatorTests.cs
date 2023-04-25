using FilmFreakApi.RestApi.Validators;
using Microsoft.AspNetCore.Http;

public class FileValidatorTests
{
    [Fact]
    public void Validate_FileMatchesTheSpec_ValidationPassesWithoutExceptions()
    {
        var specs = FileValidationSpecsBuilder.Build(FileUploadType.Jpeg);
        var fileData = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0xFF, 0xFF, 0xFF };
        var memoryStream = new MemoryStream(fileData);
        var formFile = new FormFile(memoryStream, 0, fileData.Length, "file", "file.jpeg")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpeg"
        };
        var validator = new ImageFileValidator(specs);
        validator.Validate(formFile);
    }
}