using FilmFreakApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FilmFreakApi.Infrastructure.Persistence;

public class FilmFreakContext : DbContext
{
    public FilmFreakContext(DbContextOptions<FilmFreakContext> options)
        : base(options) { }

    public required DbSet<Release> Releases { get; set; }
}