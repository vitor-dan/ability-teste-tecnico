
using IbgeRpa.Domain.Entities;
using IbgeRpa.Domain.Enums;
using IbgeRpa.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IbgeRpa.Infrastructure.Data;

public class IndicadorRepository : IIndicadorRepository
{
    private readonly AppDbContext _db;

    public IndicadorRepository(AppDbContext db) => _db = db;

    public async Task SaveOrUpdateAsync(Indicador indicador, CancellationToken ct = default)
    {
        var existing = await _db.Indicadores
            .FirstOrDefaultAsync(x => x.Nome == indicador.Nome, ct);

        if (existing is null)
            _db.Indicadores.Add(indicador);
        else
        {
            existing.Ultimo          = indicador.Ultimo;
            existing.Anterior        = indicador.Anterior;
            existing.Variacao12Meses = indicador.Variacao12Meses;
            existing.VariacaoNoAno   = indicador.VariacaoNoAno;
            existing.Categoria       = indicador.Categoria;
            existing.ColetadoEm      = indicador.ColetadoEm;
        }

        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<Indicador>> GetAllAsync(CancellationToken ct = default)
        => await _db.Indicadores.OrderBy(x => x.Categoria).ThenBy(x => x.Nome).ToListAsync(ct);

    public async Task<Indicador?> GetByNomeAsync(string nome, CancellationToken ct = default)
        => await _db.Indicadores.FirstOrDefaultAsync(x => x.Nome == nome, ct);

    public async Task<IReadOnlyList<Indicador>> GetByCategoriaAsync(
        CategoriaIndicador categoria, CancellationToken ct = default)
        => await _db.Indicadores
            .Where(x => x.Categoria == categoria)
            .OrderBy(x => x.Nome)
            .ToListAsync(ct);
}