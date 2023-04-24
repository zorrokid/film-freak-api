namespace FilmFreakApi.RestApi.Validators;

public interface IFileValidator
{
    void Validate(IFormFile file);
}

public class ImageFileValidator : IFileValidator
{
    private FileValidationSpecs _validationSpecs;

    public ImageFileValidator(FileValidationSpecs specs)
    {
        _validationSpecs = specs;
    }

    public void Validate(IFormFile file)
    {
        if (file.ContentType != _validationSpecs.ContentType)
        {
            throw new FileValidationException($"File contentType not matching assumed content type");
        }
        var fileExtension = Path.GetExtension(file.FileName);
        if (!_validationSpecs.Extensions.Contains(fileExtension.ToLowerInvariant()))
        {
            throw new FileValidationException($"File extension not supported");
        }
        if (file.Length > _validationSpecs.MaxSizeInBytes)
        {
            throw new FileValidationException($"Too large file");
        }
        if (!IsValidSignature(file))
        {
            throw new FileValidationException("Invalid signature");
        }
    }

    private bool IsValidSignature(IFormFile file)
    {
        using var reader = new BinaryReader(file.OpenReadStream());
        var signatureLenght = _validationSpecs.Signature.Max(s => s.Length);
        var headerBytes = reader.ReadBytes(signatureLenght);
        return _validationSpecs.Signature.Any(signature
            => headerBytes.Take(signature.Length).SequenceEqual(signature));
    }

}