using IbgeRpa.Infrastructure.Data;
using IbgeRpa.Infrastructure.Extensions;
using IbgeRpa.Worker.Configuration;
using IbgeRpa.Worker.Services;
using IbgeRpa.Worker.Workers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http.Resilience;
using Polly;

var builder = Host.CreateApplicationBuilder(args);

var dbPath = builder.Configuration["Database:Path"] ?? "./ibge.db";

builder.Services.AddInfrastructure($"Data Source={dbPath}");

builder.Services.Configure<ScraperOptions>(
    builder.Configuration.GetSection("Scraper"));

builder.Services
    .AddHttpClient("ibge", client =>
    {
        client.BaseAddress = new Uri("https://www.ibge.gov.br");
        client.DefaultRequestHeaders.Add(
            "User-Agent", "Mozilla/5.0 (compatible; IbgeRpa/1.0)");
        client.Timeout = TimeSpan.FromSeconds(30);
    })
    .AddResilienceHandler("retry", pipeline =>
    {
        pipeline.AddRetry(new HttpRetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            BackoffType = DelayBackoffType.Exponential,
            Delay = TimeSpan.FromSeconds(2),
            OnRetry = args =>
            {
                Console.WriteLine($"[Retry {args.AttemptNumber + 1}] Aguardando {args.RetryDelay.TotalSeconds}s...");
                return ValueTask.CompletedTask;
            }
        });
    });

builder.Services.AddScoped<IPageFetcher, HttpPageFetcher>();
builder.Services.AddScoped<IIndicadorParser, IbgeIndicadorParser>();
builder.Services.AddScoped<IbgeScraperService>();
builder.Services.AddHostedService<ScraperBackgroundWorker>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    db.Database.ExecuteSqlRaw("PRAGMA journal_mode=WAL;");
}

await host.RunAsync();