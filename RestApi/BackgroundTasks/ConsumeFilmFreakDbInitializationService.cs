using FilmFreakApi.Infrastructure.Persistence;

namespace FilmFreakApi.RestApi.BackgroundTasks;
internal class ConsumeFilmFreakDbInitializationService : BackgroundService
{
    private readonly IServiceProvider _services;

    public ConsumeFilmFreakDbInitializationService(IServiceProvider services)
    {
        _services = services;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await DoWork(stoppingToken);
    }

    private async Task DoWork(CancellationToken stoppingToken)
    {
        using (var scope = _services.CreateAsyncScope())
        {
            var filmFreakDbInitService = scope.ServiceProvider
                .GetRequiredService<IFilmFreakDbInitializationService>();
            var configuration = scope.ServiceProvider
                .GetRequiredService<IConfiguration>();
            await filmFreakDbInitService.Initialize();
        }
    }
}