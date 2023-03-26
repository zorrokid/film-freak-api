using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FilmFreakApi.Infrastructure.Persistence;

public interface IFilmFreakDbInitializationService
{
    Task Initialize();
}

public class FilmFreakDbInitializationService : IFilmFreakDbInitializationService
{
    private readonly ILogger<FilmFreakDbInitializationService> _logger;
    private readonly FilmFreakContext _dbContext;

    public FilmFreakDbInitializationService(
        ILogger<FilmFreakDbInitializationService> logger,
        FilmFreakContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Initialize()
    {
        _logger.LogInformation("Start initializing AuthDb.");
        _logger.LogInformation("Running migrations.");
        await _dbContext.Database.MigrateAsync();
        _logger.LogInformation("Finished initializing AuthDb.");
    }
}