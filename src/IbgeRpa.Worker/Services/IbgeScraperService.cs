using IbgeRpa.Domain.Interfaces;

namespace IbgeRpa.Worker.Services;

public sealed class IbgeScraperService
{
    private readonly IPageFetcher _fetcher;
    private readonly IIndicadorParser _parser;
    private readonly IIndicadorRepository _repo;
    private readonly ILogger<IbgeScraperService> _logger;

    private const string TargetUrl = "/indicadores.html";

    public IbgeScraperService(
        IPageFetcher fetcher,
        IIndicadorParser parser,
        IIndicadorRepository repo,
        ILogger<IbgeScraperService> logger)
    {
        _fetcher = fetcher;
        _parser  = parser;
        _repo    = repo;
        _logger  = logger;
    }

    public async Task ExecuteAsync(CancellationToken ct)
    {
        _logger.LogInformation("Iniciando coleta...");

        var html = await _fetcher.FetchAsync(TargetUrl, ct);

        if (html is null)
        {
            _logger.LogWarning("Coleta abortada: não foi possível obter o HTML.");
            return;
        }

        var indicadores = _parser.Parse(html);

        foreach (var item in indicadores)
        {
            await _repo.SaveOrUpdateAsync(item, ct);
            _logger.LogInformation(
                "[{Categoria}] {Nome} — Último: {Ultimo}",
                item.Categoria, item.Nome, item.Ultimo);
        }

        _logger.LogInformation(
            "Coleta concluída. {Count} indicador(es) atualizados.", indicadores.Count);
    }
}