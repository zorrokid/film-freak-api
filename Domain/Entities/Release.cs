namespace FilmFreakApi.Domain.Entities;

public class Release
{
    public long Id { get; set; }
    public string? Title { get; set; }
    public string? Barcode { get; set; }
    public string? ExternalId { get; set; }
}