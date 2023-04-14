using FilmFreakApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FilmFreakApi.Infrastructure.Persistence;

public class FilmFreakContext : DbContext
{
    public FilmFreakContext(DbContextOptions<FilmFreakContext> options)
        : base(options) { }

    public DbSet<Release> Releases => Set<Release>();
    public DbSet<CollectionItem> CollectionItems => Set<CollectionItem>();
}