using FilmFreakApi.Domain.Entities;

namespace FilmFreakApi.Application.Interfaces;

public interface IReleaseRepository
{
    Task<Release?> GetByExternalId(string externalId);
    Task Update(IEnumerable<Release> releases);
    Task Add(IEnumerable<Release> releases);
    Task AddAsync(Release release);
    Task<IEnumerable<Release>> GetReleasesAsync();
    Task<Release?> GetReleaseAsync(long releaseId);
    Task Remove(Release release);
}