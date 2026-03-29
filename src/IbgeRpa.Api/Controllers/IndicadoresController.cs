using IbgeRpa.Domain.Enums;
using IbgeRpa.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IbgeRpa.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IndicadoresController : ControllerBase
{
    private readonly IIndicadorRepository _repo;

    public IndicadoresController(IIndicadorRepository repo) => _repo = repo;

    /// <summary>Retorna todos os indicadores coletados pelo RPA.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var items = await _repo.GetAllAsync(ct);
        return items.Count == 0 ? NoContent() : Ok(items);
    }

    /// <summary>Retorna indicadores filtrados por categoria.</summary>
    [HttpGet("categoria/{categoria}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByCategoria(string categoria, CancellationToken ct)
    {
        if (!Enum.TryParse<CategoriaIndicador>(categoria, ignoreCase: true, out var categoriaEnum))
            return BadRequest($"Categoria inválida. Use: {string.Join(", ", Enum.GetNames<CategoriaIndicador>())}");

        var items = await _repo.GetByCategoriaAsync(categoriaEnum, ct);
        return items.Count == 0 ? NoContent() : Ok(items);
    }

    /// <summary>Retorna um indicador específico pelo nome.</summary>
    [HttpGet("{nome}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByNome(string nome, CancellationToken ct)
    {
        var item = await _repo.GetByNomeAsync(nome, ct);
        return item is null ? NotFound() : Ok(item);
    }
}