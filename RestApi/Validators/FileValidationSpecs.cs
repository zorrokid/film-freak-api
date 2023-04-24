namespace FilmFreakApi.RestApi.Validators;

public record FileValidationSpecs(
    String ContentType,
    List<String> Extensions,
    List<Byte[]> Signature,
    int MaxSizeInBytes
);