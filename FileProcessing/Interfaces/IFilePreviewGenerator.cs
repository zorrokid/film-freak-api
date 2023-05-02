namespace FilmFreakApi.FileProcessing.Interfaces;

public interface IFilePreviewGenerator
{
    byte[] Generate(MemoryStream stream);
}

