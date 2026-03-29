using AngleSharp;
using AngleSharp.Dom;
using IbgeRpa.Domain.Entities;
using IbgeRpa.Domain.Enums;

namespace IbgeRpa.Worker.Services;

public sealed class IbgeIndicadorParser : IIndicadorParser
{
    private readonly ILogger<IbgeIndicadorParser> _logger;

    public IbgeIndicadorParser(ILogger<IbgeIndicadorParser> logger)
        => _logger = logger;

    public IReadOnlyList<Indicador> Parse(string html)
    {
        var config = AngleSharp.Configuration.Default;
        var context = BrowsingContext.New(config);
        var document = context.OpenAsync(r => r.Content(html)).GetAwaiter().GetResult();

        var now = DateTime.UtcNow;
        var result = new List<Indicador>();

        var tables = document.QuerySelectorAll("table");

        foreach (var table in tables)
        {
            var categoria = DetectarCategoria(table);

            foreach (var row in table.QuerySelectorAll("tr"))
            {
                var tds = row.QuerySelectorAll("td");
                var ths = row.QuerySelectorAll("th");

                if (ths.Length != 1 || (tds.Length != 2 && tds.Length != 4))
                    continue;

                var nome = LimparNome(ths[0].TextContent);

                if (string.IsNullOrWhiteSpace(nome)) continue;

                result.Add(new Indicador
                {
                    Nome            = nome,
                    Categoria       = categoria,
                    Ultimo          = LimparValor(tds[0].TextContent),
                    Anterior        = LimparValor(tds[1].TextContent),
                    Variacao12Meses = tds.Length == 4 ? LimparValor(tds[2].TextContent) : null,
                    VariacaoNoAno   = tds.Length == 4 ? LimparValor(tds[3].TextContent) : null,
                    Fonte           = "IBGE /indicadores.html",
                    ColetadoEm      = now
                });
            }
        }

        if (result.Count == 0)
            _logger.LogError("Nenhum indicador encontrado. O layout da página pode ter mudado.");
        else
            _logger.LogInformation("{Count} indicadores extraídos.", result.Count);

        return result;
    }

    private static CategoriaIndicador DetectarCategoria(IElement table)
    {
        var element = table.PreviousElementSibling;

        while (element is not null)
        {
            var texto = element.TextContent.ToLower();

            if (texto.Contains("econôm")) return CategoriaIndicador.Economico;
            if (texto.Contains("socia")) return CategoriaIndicador.Social;
            if (texto.Contains("agropec")) return CategoriaIndicador.Agropecuario;

            element = element.PreviousElementSibling;
        }

        var header = table.QuerySelector("thead, caption")?.TextContent.ToLower() ?? "";

        if (header.Contains("econôm")) return CategoriaIndicador.Economico;
        if (header.Contains("socia")) return CategoriaIndicador.Social;
        if (header.Contains("agropec")) return CategoriaIndicador.Agropecuario;

        return CategoriaIndicador.Economico;
    }

    private static string LimparNome(string texto) =>
        string.Join(" ", texto
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Trim())
            .Where(l => l.Length > 0));

    private static string LimparValor(string texto)
    {
        var linhas = texto
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Trim())
            .Where(l => l.Length > 0)
            .ToList();

        return linhas.Count > 1
            ? string.Join(" ", linhas.Skip(1))
            : linhas.FirstOrDefault() ?? "";
    }
}