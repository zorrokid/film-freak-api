using FilmFreakApi.Application.Models;
using FilmFreakApi.Domain.Entities;

namespace FilmFreakApi.Application.Interfaces;

public interface IReleaseService
{
    Task AddReleaseAsync(ReleaseDTO release);
    Task<IEnumerable<Release>> GetReleasesAsync();
    Task<Release?> GetReleaseAsync(long id);
    Task Remove(Release release);
}