namespace IbgeRpa.Worker.Services;

public interface IPageFetcher
{
    Task<string?> FetchAsync(string url, CancellationToken ct = default);
}