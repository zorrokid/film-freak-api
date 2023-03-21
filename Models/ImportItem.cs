public record ImportItem
{
    public ImportItem(
        string externalId,
        string barcode,
        string originalName,
        string localName,
        string mediaType,
        string releaseCountry,
        string releaseId)
    {
        ExternalId = externalId;
        Barcode = barcode;
        OriginalName = originalName;
        LocalName = localName;
        MediaType = mediaType;
        ReleaseCountry = releaseCountry;
        ReleaseId = releaseId;
    }

    public string ExternalId { get; }
    public string Barcode { get; }
    public string OriginalName { get; }
    public string LocalName { get; }
    public string MediaType { get; }
    public string ReleaseCountry { get; }
    public string ReleaseId { get; }
}
