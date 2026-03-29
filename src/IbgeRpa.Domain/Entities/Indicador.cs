using IbgeRpa.Domain.Enums;

namespace IbgeRpa.Domain.Entities;

public class Indicador
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public CategoriaIndicador Categoria { get; set; }
    public string Ultimo { get; set; } = string.Empty;
    public string Anterior { get; set; } = string.Empty;
    public string? Variacao12Meses { get; set; }
    public string? VariacaoNoAno { get; set; }
    public string Fonte { get; set; } = string.Empty;
    public DateTime ColetadoEm { get; set; }
}