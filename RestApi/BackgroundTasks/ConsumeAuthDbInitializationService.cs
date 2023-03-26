using FilmFreakApi.Auth.Options;
using FilmFreakApi.Auth.Services;

namespace FilmFreakApi.RestApi.BackgroundTasks;

internal class ConsumeAuthDbInitializationService : BackgroundService
{
    private readonly IServiceProvider _services;

    public ConsumeAuthDbInitializationService(IServiceProvider services)
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
            var authDbInitService = scope.ServiceProvider
                .GetRequiredService<IAuthDbInitializationService>();
            var configuration = scope.ServiceProvider
                .GetRequiredService<IConfiguration>();
            var authOptions = configuration
                .GetSection(AdminCredentialsOptions.AdminCredentials)
                .Get<AdminCredentialsOptions>();
            if (authOptions == null)
            {
                throw new Exception("AdminCredentils options not configured.");
            }

            await authDbInitService.Initialize(authOptions);
        }
    }
}