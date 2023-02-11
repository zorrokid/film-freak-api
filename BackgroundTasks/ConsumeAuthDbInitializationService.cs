//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-7.0&tabs=visual-studio
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
        using(var scope = _services.CreateAsyncScope())
        {
            var authDbInitService = scope.ServiceProvider
                .GetRequiredService<IAuthDbInitializationService>();
            await authDbInitService.Initialize();
        }
    }
} 