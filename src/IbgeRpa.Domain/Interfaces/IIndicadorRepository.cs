using IbgeRpa.Domain.Entities;
using IbgeRpa.Domain.Enums;

namespace IbgeRpa.Domain.Interfaces;

public interface IIndicadorRepository
{
    Task SaveOrUpdateAsync(Indicador indicador, CancellationToken ct = default);
    Task<IReadOnlyList<Indicador>> GetAllAsync(CancellationToken ct = default);
    Task<Indicador?> GetByNomeAsync(string nome, CancellationToken ct = default);
    Task<IReadOnlyList<Indicador>> GetByCategoriaAsync(CategoriaIndicador categoria, CancellationToken ct = default);
}