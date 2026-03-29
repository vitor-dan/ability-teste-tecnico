using IbgeRpa.Worker.Configuration;
using IbgeRpa.Worker.Services;
using Microsoft.Extensions.Options;

namespace IbgeRpa.Worker.Workers;

public sealed class ScraperBackgroundWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ScraperBackgroundWorker> _logger;
    private readonly ScraperOptions _options;

    public ScraperBackgroundWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<ScraperBackgroundWorker> logger,
        IOptions<ScraperOptions> options)
    {
        _scopeFactory = scopeFactory;
        _logger       = logger;
        _options      = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Worker iniciado. Intervalo: {Interval}min", _options.IntervalMinutes);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var scraper     = scope.ServiceProvider.GetRequiredService<IbgeScraperService>();
                await scraper.ExecuteAsync(stoppingToken);
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no ciclo de scraping.");
            }

            await Task.Delay(
                TimeSpan.FromMinutes(_options.IntervalMinutes),
                stoppingToken);
        }

        _logger.LogInformation("Worker encerrado.");
    }
}