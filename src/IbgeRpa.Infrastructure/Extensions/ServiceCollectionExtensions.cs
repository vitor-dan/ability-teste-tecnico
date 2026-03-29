using IbgeRpa.Domain.Interfaces;
using IbgeRpa.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IbgeRpa.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlite(connectionString));

        services.AddScoped<IIndicadorRepository, IndicadorRepository>();

        return services;
    }
}