public interface IFileUploadPostProcessor
{
    byte[] Process(MemoryStream stream);
}
