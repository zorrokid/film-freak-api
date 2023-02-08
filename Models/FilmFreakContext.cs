using Microsoft.EntityFrameworkCore;

namespace FilmFreakApi.Models;

public class FilmFreakContext : DbContext
{
    public FilmFreakContext(DbContextOptions<FilmFreakContext> options)
        : base(options) { }

    public required DbSet<Release> Releases { get; set; }
}