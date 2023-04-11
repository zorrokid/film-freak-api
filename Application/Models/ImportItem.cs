namespace FilmFreakApi.Application.Models;

public record ImportItem(
    string ExternalId,
    string Barcode,
    string OriginalName,
    string LocalName,
    string MediaType,
    string ReleaseCountry,
    string ReleaseId);
