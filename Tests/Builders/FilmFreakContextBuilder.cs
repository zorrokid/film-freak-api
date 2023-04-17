using System.Data.Common;
using FilmFreakApi.Domain.Entities;
using FilmFreakApi.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Tests.Builders;

public class FilmFreakContextBuilder
{
    private readonly DbConnection _connection;
    private readonly DbContextOptions<FilmFreakContext> _contextOptions;
    private readonly FilmFreakContext _context;

    public FilmFreakContextBuilder()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        // See about opening anc closing connection: 
        // https://learn.microsoft.com/en-gb/ef/core/testing/testing-without-the-database#sqlite-in-memory
        _connection.Open();
        _contextOptions = new DbContextOptionsBuilder<FilmFreakContext>()
            .UseSqlite(_connection)
            .Options;
        _context = new FilmFreakContext(_contextOptions);
    }

    FilmFreakContext CreateContext => new FilmFreakContext(_contextOptions);

    public void Dispose() => _connection.Dispose();

    public FilmFreakContextBuilder WithRelease(Release release)
    {
        _context.Releases.Add(release);
        return this;
    }

    public FilmFreakContext Build()
    {
        _context.SaveChanges();
        return _context;
    }


}