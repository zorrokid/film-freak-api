using FilmFreakApi.Domain.Entities;

namespace FilmFreakApi.Application.Models;

public record ReleaseDTO(long? Id, string? ExternalId, string? Title, string? Barcode)
{
    public static explicit operator Release(ReleaseDTO releaseDTO)
    {
        return new Release
        {
            ExternalId = releaseDTO.ExternalId,
            Name = releaseDTO.Title,
            Barcode = releaseDTO.Barcode
        };
    }

    public static explicit operator ReleaseDTO(Release release)
    {
        return new ReleaseDTO(release.Id, release.ExternalId, release.Name, release.Barcode);
    }
}
