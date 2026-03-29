namespace IbgeRpa.Worker.Services;

public sealed class HttpPageFetcher : IPageFetcher
{
    private readonly IHttpClientFactory _http;
    private readonly ILogger<HttpPageFetcher> _logger;

    public HttpPageFetcher(IHttpClientFactory http, ILogger<HttpPageFetcher> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<string?> FetchAsync(string url, CancellationToken ct = default)
    {
        try
        {
            var client = _http.CreateClient("ibge");
            return await client.GetStringAsync(url, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao buscar {Url}", url);
            return null;
        }
    }
}