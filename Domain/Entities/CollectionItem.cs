namespace FilmFreakApi.Domain.Entities;

public enum Condition
{
    Unknown,
    Bad,
    Poor,
    Fair,
    Good,
    Excellent,
    Mint,
}

public enum CollectionStatus
{
    Unknown,
    Own,
    Ordered,
    Trade,
    Owned
}

public class CollectionItem : EntityBase
{
    public long ReleaseId { get; set; }
    public Condition Condition { get; set; }
    public CollectionStatus CollectionStatus { get; set; }
    public Release Release { get; set; } = null!;
    public string? ExternalId { get; set; }
    public string UserId { get; set; } = null!;
}