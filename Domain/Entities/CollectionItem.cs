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

public class CollectionItem
{
    public long Id { get; set; }
    public long ReleaseId { get; set; }
    public Condition Condition { get; set; }
    public CollectionStatus CollectionStatus { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime ModifiedTime { get; set; }
}