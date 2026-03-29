using IbgeRpa.Domain.Entities;

namespace IbgeRpa.Worker.Services;

public interface IIndicadorParser
{
    IReadOnlyList<Indicador> Parse(string html);
}