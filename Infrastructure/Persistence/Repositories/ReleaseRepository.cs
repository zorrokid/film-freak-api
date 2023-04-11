using FilmFreakApi.Application.Interfaces;
using FilmFreakApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FilmFreakApi.Infrastructure.Persistence.Repositories;

public class ReleaseRepository : IReleaseRepository
{
    private readonly FilmFreakContext _context;

    public ReleaseRepository(FilmFreakContext context)
    {
        _context = context;
    }

    public async Task<Release?> GetByExternalId(string externalId, string userId)
    {
        return await _context.Releases
            .FirstOrDefaultAsync(r =>
                r.ExternalId == externalId &&
                r.UserId == userId &&
                !r.IsShared);
    }

    public async Task Add(IEnumerable<Release> releases)
    {
        await _context.Releases.AddRangeAsync(releases);
        await _context.SaveChangesAsync();
    }

    public async Task Update(IEnumerable<Release> releases)
    {
        _context.Releases.AttachRange(releases);
        await _context.SaveChangesAsync();
    }

    public async Task AddAsync(Release release)
    {
        _context.Releases.Add(release);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Release>> GetReleasesAsync()
    {
        return await _context.Releases.ToListAsync();
    }

    public async Task<Release?> GetReleaseAsync(long id)
    {
        return await _context.Releases.FindAsync(id);
    }

    public async Task Remove(Release release)
    {
        _context.Releases.Remove(release);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<string>> GetExternalIdsAsync(string userId)
    {
        return await _context.Releases
            .Where(r => r.UserId == userId && r.ExternalId != null && !r.IsShared)
            .Select(r => r.ExternalId!)
            .Distinct()
            .ToListAsync();
    }
}