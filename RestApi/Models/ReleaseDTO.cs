namespace FilmFreakApi.Models;

public class ReleaseDTO
{
    public long Id { get; set; }
    public string? ExternalId { get; set; }
    public string? Title { get; set; }
    public string? Barcode { get; set; }
}