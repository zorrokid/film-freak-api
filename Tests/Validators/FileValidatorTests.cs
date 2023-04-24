using FilmFreakApi.RestApi.Validators;
using Microsoft.AspNetCore.Http;

public class FileValidatorTests
{
    [Fact]
    public void Validate_FileMatchesTheSpec_ValidationPassesWithoutExceptions()
    {
        var specs = new FileValidationSpecs
        (
            ContentType: "image/jpeg",
            Extensions: new List<string> { ".jpg", ".jpeg" },
            MaxSizeInBytes: 10,
            Signature: new List<byte[]>
            {
                new byte[] {0xFF, 0xD8, 0xFF, 0xE0},
                new byte[] {0xFF, 0xD8, 0xFF, 0xE2},
                new byte[] {0xFF, 0xD8, 0xFF, 0xE3}
            }
        );

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