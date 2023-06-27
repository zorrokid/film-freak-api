using FilmFreakApi.Application.Interfaces;
using FilmFreakApi.Application.Models;
using FilmFreakApi.Domain.Entities;

namespace FilmFreakApi.Application.Services;

public class ReleaseService : IReleaseService
{
    private readonly IReleaseRepository _releaseRepository;

    public ReleaseService(IReleaseRepository releaseRepository)
    {
        _releaseRepository = releaseRepository;
    }

    public async Task AddReleaseAsync(ReleaseDTO release)
    {
        await _releaseRepository.AddAsync((Release)release);
    }

    public async Task<Release?> GetReleaseAsync(long id)
    {
        return await _releaseRepository.GetReleaseAsync(id);
    }

    public async Task<IEnumerable<Release>> GetReleasesAsync()
    {
        return await _releaseRepository.GetReleasesAsync();
    }

    public async Task Remove(Release release)
    {
        await _releaseRepository.Remove(release);
    }
}