namespace FilmFreakApi.Domain.Entities;

public class Release
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? Barcode { get; set; }
    public string? ExternalId { get; set; }
    public bool IsShared { get; set; }
    public string? UserId { get; set; }
    public ICollection<CollectionItem> CollectionItems { get; } = new List<CollectionItem>();
}